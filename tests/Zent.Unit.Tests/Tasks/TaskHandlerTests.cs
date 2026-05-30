using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Zent.Application.Features.Tasks.AddTask;
using Zent.Common.Enums;
using Zent.Common.Exceptions;
using Zent.Data;
using Zent.Data.Entities;
using Zent.Unit.Tests.Builders;

namespace Zent.Unit.Tests.Tasks;

public sealed class TaskHandlerTests
{
    [Fact]
    public async Task AddTask_ShouldCreateTask_WhenUserIsTeamMember()
    {
        await using var db = TestDbContextFactory.Create();
        var user = await SeedUserAsync(db);
        var (board, column) = await SeedBoardWithColumnAsync(db, user, TeamRole.Member);
        var handler = new AddTaskHandler(db);
        var command = new AddTaskCommand(
            user.Id,
            board.Id,
            column.Id,
            "Implement tests",
            "Cover key handlers",
            TaskPriority.High,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
            user.Id);

        var taskId = await handler.Handle(command, TestContext.Current.CancellationToken);

        var task = await db.Tasks.SingleAsync(x => x.Id == taskId, TestContext.Current.CancellationToken);

        task.ColumnId.Should().Be(column.Id);
        task.CreatorId.Should().Be(user.Id);
        task.AssigneeId.Should().Be(user.Id);
        task.Title.Should().Be("Implement tests");
        task.Priority.Should().Be(TaskPriority.High);
        task.Order.Should().Be(1);
    }

    [Fact]
    public async Task AddTask_ShouldThrow_WhenUserIsNotTeamMember()
    {
        await using var db = TestDbContextFactory.Create();
        var user = await SeedUserAsync(db);
        var owner = await SeedUserAsync(db, $"owner-{Guid.NewGuid()}@example.com");
        var (board, column) = await SeedBoardWithColumnAsync(db, owner, TeamRole.Owner);
        var handler = new AddTaskHandler(db);

        var act = () => handler.Handle(
            new AddTaskCommand(user.Id, board.Id, column.Id, "Implement tests", null, TaskPriority.Low, null, null),
            TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<TeamAccessDeniedException>();
    }

    [Fact]
    public async Task AddTask_ShouldThrow_WhenColumnDoesNotBelongToBoard()
    {
        await using var db = TestDbContextFactory.Create();
        var user = await SeedUserAsync(db);
        var (board, _) = await SeedBoardWithColumnAsync(db, user, TeamRole.Member);
        var (_, otherColumn) = await SeedBoardWithColumnAsync(db, user, TeamRole.Member);
        var handler = new AddTaskHandler(db);

        var act = () => handler.Handle(
            new AddTaskCommand(user.Id, board.Id, otherColumn.Id, "Implement tests", null, TaskPriority.Low, null, null),
            TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<ColumnNotFoundException>();
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

    private static async Task<(BoardEntity Board, ColumnEntity Column)> SeedBoardWithColumnAsync(
        ZentDbContext db,
        UserEntity user,
        TeamRole role)
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

        db.ProjectMembers.Add(new ProjectMemberBuilder().ForUser(user).ForProject(project).Build());
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var board = new BoardBuilder().ForProject(project).Build();
        db.Boards.Add(board);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        board = await db.Boards
            .AsNoTracking()
            .SingleAsync(x => x.ProjectId == project.Id && x.Name == board.Name, TestContext.Current.CancellationToken);

        var column = new ColumnBuilder().ForBoard(board).WithTitle("Ready").Build();
        db.Columns.Add(column);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        column = await db.Columns
            .AsNoTracking()
            .SingleAsync(x => x.BoardId == board.Id && x.Title == "Ready", TestContext.Current.CancellationToken);

        return (board, column);
    }
}
