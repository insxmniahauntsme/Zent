using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Zent.API.Endpoints.Boards.AddBoard;
using Zent.Common.Enums;
using Zent.Data;
using Zent.Data.Entities;
using Zent.Integration.Tests.Builders;
using Zent.Integration.Tests.Infrastructure;
using Zent.Integration.Tests.Infrastructure.Fixtures;

namespace Zent.Integration.Tests.Boards;

[Collection("Integration")]
public sealed class BoardEndpointTests(TestHostFixture host)
    : IntegrationTestBase(host)
{
    [Fact]
    public async Task AddBoard_ShouldCreateBoardWithDefaultColumns_WhenUserCanManageTeam()
    {
        var user = await GetTestUserAsync();
        var project = await SeedProjectAsync(user, TeamRole.Owner, addProjectMembership: true);
        var client = await Host.HttpClients.CreateAuthenticatedAsync();
        var request = new AddBoardRequest("Sprint Board", "Delivery board");

        var response = await client.PostAsJsonAsync(
            $"/api/projects/{project.Id}/boards",
            request,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        await using var stream = await response.Content.ReadAsStreamAsync(TestContext.Current.CancellationToken);
        using var payload = await JsonDocument.ParseAsync(stream, cancellationToken: TestContext.Current.CancellationToken);
        var boardId = payload.RootElement.GetProperty("id").GetGuid();

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ZentDbContext>();

        var board = await db.Boards.SingleAsync(x => x.Id == boardId, TestContext.Current.CancellationToken);
        var columns = await db.Columns
            .Where(x => x.BoardId == boardId)
            .OrderBy(x => x.Order)
            .Select(x => x.Title)
            .ToListAsync(TestContext.Current.CancellationToken);

        board.ProjectId.Should().Be(project.Id);
        board.Name.Should().Be(request.Name);
        columns.Should().Equal("Backlog", "To Do", "In Progress", "Review", "Done");
    }

    [Fact]
    public async Task GetBoard_ShouldReturnColumnsAndTasks_WhenUserIsProjectMember()
    {
        var user = await GetTestUserAsync();
        var project = await SeedProjectAsync(user, TeamRole.Owner, addProjectMembership: true);
        BoardEntity board;

        using (var scope = Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ZentDbContext>();

            board = new BoardBuilder().ForProject(project).WithName("Feature Board").Build();

            db.Boards.Add(board);
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);

            board = await db.Boards
                .AsNoTracking()
                .SingleAsync(
                    x => x.ProjectId == project.Id && x.Name == "Feature Board",
                    TestContext.Current.CancellationToken);

            var column = new ColumnBuilder().ForBoard(board).WithTitle("Ready").Build();

            db.Columns.Add(column);
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);

            column = await db.Columns
                .AsNoTracking()
                .SingleAsync(
                    x => x.BoardId == board.Id && x.Title == "Ready",
                    TestContext.Current.CancellationToken);

            var task = new TaskBuilder().ForColumn(column).CreatedBy(user).WithTitle("Ship endpoint tests").Build();

            db.Tasks.Add(task);
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        var client = await Host.HttpClients.CreateAuthenticatedAsync();

        var response = await client.GetAsync($"/api/boards/{board.Id}", TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(TestContext.Current.CancellationToken);
        using var payload = await JsonDocument.ParseAsync(stream, cancellationToken: TestContext.Current.CancellationToken);
        var boardPayload = payload.RootElement.GetProperty("board");
        var columns = boardPayload.GetProperty("columns").EnumerateArray().ToList();

        boardPayload.GetProperty("name").GetString().Should().Be("Feature Board");
        columns.Should().ContainSingle();
        columns.Single().GetProperty("tasks").EnumerateArray().ToList()
            .Should().ContainSingle(x => x.GetProperty("title").GetString() == "Ship endpoint tests");
    }

    [Fact]
    public async Task AddBoard_ShouldReturnForbidden_WhenUserCannotManageTeam()
    {
        var user = await GetTestUserAsync();
        var project = await SeedProjectAsync(user, TeamRole.Member, addProjectMembership: true);
        var client = await Host.HttpClients.CreateAuthenticatedAsync();
        var request = new AddBoardRequest("Sprint Board", null);

        var response = await client.PostAsJsonAsync(
            $"/api/projects/{project.Id}/boards",
            request,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<ProjectEntity> SeedProjectAsync(
        UserEntity user,
        TeamRole role,
        bool addProjectMembership)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ZentDbContext>();

        var team = new TeamBuilder().WithOwner(user).Build();

        db.Teams.Add(team);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        team = await db.Teams
            .AsNoTracking()
            .SingleAsync(
                x => x.OwnerId == user.Id && x.Name == team.Name,
                TestContext.Current.CancellationToken);

        db.TeamMembers.Add(new TeamMemberBuilder().ForUser(user).ForTeam(team).WithRole(role).Build());
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var project = new ProjectBuilder().ForTeam(team).Build();

        db.Projects.Add(project);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        project = await db.Projects
            .AsNoTracking()
            .SingleAsync(
                x => x.TeamId == team.Id && x.Name == project.Name,
                TestContext.Current.CancellationToken);

        if (addProjectMembership)
        {
            db.ProjectMembers.Add(new ProjectMemberBuilder().ForUser(user).ForProject(project).Build());
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        return project;
    }
}
