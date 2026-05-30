using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Zent.Application.Features.Boards.AddBoard;
using Zent.Application.Features.Boards.GetBoard;
using Zent.Common.Enums;
using Zent.Common.Exceptions;
using Zent.Data;
using Zent.Data.Entities;
using Zent.Unit.Tests.Builders;

namespace Zent.Unit.Tests.Boards;

public sealed class BoardHandlerTests
{
    [Fact]
    public async Task AddBoard_ShouldCreateBoardWithDefaultColumns_WhenUserCanManageTeam()
    {
        await using var db = TestDbContextFactory.Create();
        var user = await SeedUserAsync(db);
        var project = await SeedProjectAsync(db, user, TeamRole.Owner, addProjectMembership: true);
        var handler = new AddBoardHandler(db);

        var boardId = await handler.Handle(
            new AddBoardCommand(user.Id, project.Id, "Sprint Board", "Delivery board"),
            TestContext.Current.CancellationToken);

        var board = await db.Boards.SingleAsync(x => x.Id == boardId, TestContext.Current.CancellationToken);
        var columns = await db.Columns
            .Where(x => x.BoardId == boardId)
            .OrderBy(x => x.Order)
            .Select(x => x.Title)
            .ToListAsync(TestContext.Current.CancellationToken);

        board.ProjectId.Should().Be(project.Id);
        board.Name.Should().Be("Sprint Board");
        columns.Should().Equal("Backlog", "To Do", "In Progress", "Review", "Done");
    }

    [Fact]
    public async Task AddBoard_ShouldThrow_WhenUserCannotManageTeam()
    {
        await using var db = TestDbContextFactory.Create();
        var user = await SeedUserAsync(db);
        var project = await SeedProjectAsync(db, user, TeamRole.Member, addProjectMembership: true);
        var handler = new AddBoardHandler(db);

        var act = () => handler.Handle(
            new AddBoardCommand(user.Id, project.Id, "Sprint Board", null),
            TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<TeamAccessDeniedException>();
    }

    [Fact]
    public async Task GetBoard_ShouldReturnColumnsAndTasks_WhenUserIsProjectMember()
    {
        await using var db = TestDbContextFactory.Create();
        var user = await SeedUserAsync(db);
        var project = await SeedProjectAsync(db, user, TeamRole.Owner, addProjectMembership: true);
        var board = await SeedBoardAsync(db, project, "Feature Board");
        var column = await SeedColumnAsync(db, board, "Ready");
        await SeedTaskAsync(db, column, user, "Ship endpoint tests");
        var handler = new GetBoardHandler(db);

        var result = await handler.Handle(
            new GetBoardQuery(user.Id, board.Id),
            TestContext.Current.CancellationToken);

        result.Name.Should().Be("Feature Board");
        result.Columns.Should().ContainSingle();
        result.Columns.Single().Tasks.Should().ContainSingle(x => x.Title == "Ship endpoint tests");
    }

    [Fact]
    public async Task GetBoard_ShouldThrow_WhenUserIsNotProjectMember()
    {
        await using var db = TestDbContextFactory.Create();
        var user = await SeedUserAsync(db);
        var owner = await SeedUserAsync(db, $"owner-{Guid.NewGuid()}@example.com");
        var project = await SeedProjectAsync(db, owner, TeamRole.Owner, addProjectMembership: true);
        var board = await SeedBoardAsync(db, project, "Feature Board");
        var handler = new GetBoardHandler(db);

        var act = () => handler.Handle(
            new GetBoardQuery(user.Id, board.Id),
            TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<ProjectAccessDeniedException>();
    }

    private static async Task<UserEntity> SeedUserAsync(ZentDbContext db, string? email = null)
    {
        var user = new UserBuilder()
            .WithEmail(email ?? $"user-{Guid.NewGuid()}@example.com")
            .Build();

        db.Users.Add(user);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        return await db.Users
            .AsNoTracking()
            .SingleAsync(x => x.Email == user.Email, TestContext.Current.CancellationToken);
    }

    private static async Task<ProjectEntity> SeedProjectAsync(
        ZentDbContext db,
        UserEntity user,
        TeamRole role,
        bool addProjectMembership)
    {
        var team = new TeamBuilder().WithOwner(user).Build();
        db.Teams.Add(team);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        team = await db.Teams
            .AsNoTracking()
            .SingleAsync(x => x.OwnerId == user.Id && x.Name == team.Name, TestContext.Current.CancellationToken);

        db.TeamMembers.Add(new TeamMemberBuilder().ForUser(user).ForTeam(team).WithRole(role).Build());
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var project = new ProjectBuilder().ForTeam(team).Build();
        db.Projects.Add(project);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        project = await db.Projects
            .AsNoTracking()
            .SingleAsync(x => x.TeamId == team.Id && x.Name == project.Name, TestContext.Current.CancellationToken);

        if (addProjectMembership)
        {
            db.ProjectMembers.Add(new ProjectMemberBuilder().ForUser(user).ForProject(project).Build());
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        return project;
    }

    private static async Task<BoardEntity> SeedBoardAsync(
        ZentDbContext db,
        ProjectEntity project,
        string name)
    {
        var board = new BoardBuilder().ForProject(project).WithName(name).Build();
        db.Boards.Add(board);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        return await db.Boards
            .AsNoTracking()
            .SingleAsync(x => x.ProjectId == project.Id && x.Name == name, TestContext.Current.CancellationToken);
    }

    private static async Task<ColumnEntity> SeedColumnAsync(
        ZentDbContext db,
        BoardEntity board,
        string title)
    {
        var column = new ColumnBuilder().ForBoard(board).WithTitle(title).Build();
        db.Columns.Add(column);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        return await db.Columns
            .AsNoTracking()
            .SingleAsync(x => x.BoardId == board.Id && x.Title == title, TestContext.Current.CancellationToken);
    }

    private static async Task SeedTaskAsync(
        ZentDbContext db,
        ColumnEntity column,
        UserEntity user,
        string title)
    {
        db.Tasks.Add(new TaskBuilder()
            .ForColumn(column)
            .CreatedBy(user)
            .AssignedTo(user)
            .WithTitle(title)
            .Build());
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);
    }
}
