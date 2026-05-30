using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Zent.API.Endpoints.Tasks.AddTask;
using Zent.Common.Enums;
using Zent.Data;
using Zent.Data.Entities;
using Zent.Integration.Tests.Builders;
using Zent.Integration.Tests.Infrastructure;
using Zent.Integration.Tests.Infrastructure.Fixtures;

namespace Zent.Integration.Tests.Tasks;

[Collection("Integration")]
public sealed class TaskEndpointTests(TestHostFixture host)
    : IntegrationTestBase(host)
{
    [Fact]
    public async Task AddTask_ShouldCreateTask_WhenUserIsTeamMember()
    {
        var user = await GetTestUserAsync();
        var (board, column) = await SeedBoardWithColumnAsync(user, TeamRole.Member);
        var client = await Host.HttpClients.CreateAuthenticatedAsync();
        var request = new AddTaskRequest(
            Title: "Implement tests",
            Description: "Cover key endpoints",
            Priority: TaskPriority.High,
            UntilDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
            AssigneeId: user.Id);

        var response = await client.PostAsJsonAsync(
            $"/api/boards/{board.Id}/columns/{column.Id}/tasks",
            request,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await using var stream = await response.Content.ReadAsStreamAsync(TestContext.Current.CancellationToken);
        using var payload = await JsonDocument.ParseAsync(stream, cancellationToken: TestContext.Current.CancellationToken);
        var taskId = payload.RootElement.GetProperty("taskId").GetGuid();

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ZentDbContext>();

        var task = await db.Tasks.SingleAsync(x => x.Id == taskId, TestContext.Current.CancellationToken);

        task.ColumnId.Should().Be(column.Id);
        task.CreatorId.Should().Be(user.Id);
        task.AssigneeId.Should().Be(user.Id);
        task.Title.Should().Be(request.Title);
        task.Priority.Should().Be(TaskPriority.High);
        task.Order.Should().Be(1);
    }

    [Fact]
    public async Task AddTask_ShouldReturnBadRequest_WhenTitleIsBlank()
    {
        var user = await GetTestUserAsync();
        var (board, column) = await SeedBoardWithColumnAsync(user, TeamRole.Member);
        var client = await Host.HttpClients.CreateAuthenticatedAsync();
        var request = new AddTaskRequest(" ", null, TaskPriority.Low, null, null);

        var response = await client.PostAsJsonAsync(
            $"/api/boards/{board.Id}/columns/{column.Id}/tasks",
            request,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddTask_ShouldReturnForbidden_WhenUserIsNotTeamMember()
    {
        var owner = new UserBuilder().WithEmail($"owner-{Guid.NewGuid()}@example.com").Build();
        var (board, column) = await SeedBoardWithColumnAsync(owner, TeamRole.Owner);
        var client = await Host.HttpClients.CreateAuthenticatedAsync();
        var request = new AddTaskRequest("Implement tests", null, TaskPriority.Low, null, null);

        var response = await client.PostAsJsonAsync(
            $"/api/boards/{board.Id}/columns/{column.Id}/tasks",
            request,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<(BoardEntity Board, ColumnEntity Column)> SeedBoardWithColumnAsync(
        UserEntity user,
        TeamRole role)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ZentDbContext>();

        if (!await db.Users.AnyAsync(x => x.Id == user.Id, TestContext.Current.CancellationToken))
        {
            db.Users.Add(user);
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);

            user = await db.Users
                .AsNoTracking()
                .SingleAsync(x => x.Email == user.Email, TestContext.Current.CancellationToken);
        }

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

        db.ProjectMembers.Add(new ProjectMemberBuilder().ForUser(user).ForProject(project).Build());
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var board = new BoardBuilder().ForProject(project).Build();

        db.Boards.Add(board);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        board = await db.Boards
            .AsNoTracking()
            .SingleAsync(
                x => x.ProjectId == project.Id && x.Name == board.Name,
                TestContext.Current.CancellationToken);

        var column = new ColumnBuilder().ForBoard(board).WithTitle("Ready").Build();

        db.Columns.Add(column);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        column = await db.Columns
            .AsNoTracking()
            .SingleAsync(
                x => x.BoardId == board.Id && x.Title == column.Title,
                TestContext.Current.CancellationToken);

        return (board, column);
    }
}
