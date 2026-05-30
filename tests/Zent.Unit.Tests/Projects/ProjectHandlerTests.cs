using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Zent.Application.Features.Projects.AddProject;
using Zent.Application.Features.Projects.GetTeamProjects;
using Zent.Common.Enums;
using Zent.Common.Exceptions;
using Zent.Data;
using Zent.Data.Entities;
using Zent.Unit.Tests.Builders;

namespace Zent.Unit.Tests.Projects;

public sealed class ProjectHandlerTests
{
    [Fact]
    public async Task AddProject_ShouldCreateProjectAndCurrentUserMembership_WhenUserCanManageTeam()
    {
        await using var db = TestDbContextFactory.Create();
        var user = await SeedUserAsync(db);
        var team = await SeedTeamAsync(db, user, TeamRole.Owner);
        var handler = new AddProjectHandler(db);
        var command = new AddProjectCommand(user.Id, team.Id, "Roadmap", "Product roadmap", "Zent", null);

        var projectId = await handler.Handle(command, TestContext.Current.CancellationToken);

        var project = await db.Projects.SingleAsync(x => x.Id == projectId, TestContext.Current.CancellationToken);
        var membershipExists = await db.ProjectMembers.AnyAsync(
            x => x.ProjectId == projectId && x.UserId == user.Id,
            TestContext.Current.CancellationToken);

        project.TeamId.Should().Be(team.Id);
        project.Name.Should().Be("Roadmap");
        membershipExists.Should().BeTrue();
    }

    [Fact]
    public async Task AddProject_ShouldThrow_WhenUserCannotManageTeam()
    {
        await using var db = TestDbContextFactory.Create();
        var user = await SeedUserAsync(db);
        var team = await SeedTeamAsync(db, user, TeamRole.Member);
        var handler = new AddProjectHandler(db);

        var act = () => handler.Handle(
            new AddProjectCommand(user.Id, team.Id, "Roadmap", null, null, null),
            TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<TeamAccessDeniedException>();
    }

    [Fact]
    public async Task GetTeamProjects_ShouldReturnProjects_WhenUserIsTeamMember()
    {
        await using var db = TestDbContextFactory.Create();
        var user = await SeedUserAsync(db);
        var team = await SeedTeamAsync(db, user, TeamRole.Member);
        var firstProject = await SeedProjectAsync(db, team, "First", DateTime.UtcNow.AddDays(-1));
        var secondProject = await SeedProjectAsync(db, team, "Second", DateTime.UtcNow);
        db.ProjectMembers.Add(new ProjectMemberBuilder().ForUser(user).ForProject(firstProject).Build());
        db.ProjectMembers.Add(new ProjectMemberBuilder().ForUser(user).ForProject(secondProject).Build());
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        var handler = new GetTeamProjectsHandler(db);

        var result = await handler.Handle(
            new GetTeamProjectsQuery(user.Id, team.Id),
            TestContext.Current.CancellationToken);

        result.Select(x => x.Name).Should().Equal("Second", "First");
    }

    [Fact]
    public async Task GetTeamProjects_ShouldThrow_WhenUserIsNotTeamMember()
    {
        await using var db = TestDbContextFactory.Create();
        var user = await SeedUserAsync(db);
        var owner = await SeedUserAsync(db, $"owner-{Guid.NewGuid()}@example.com");
        var team = await SeedTeamAsync(db, owner, TeamRole.Owner);
        var handler = new GetTeamProjectsHandler(db);

        var act = () => handler.Handle(
            new GetTeamProjectsQuery(user.Id, team.Id),
            TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<TeamAccessDeniedException>();
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

    private static async Task<TeamEntity> SeedTeamAsync(
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

        return team;
    }

    private static async Task<ProjectEntity> SeedProjectAsync(
        ZentDbContext db,
        TeamEntity team,
        string name,
        DateTime createdAt)
    {
        var project = new ProjectBuilder()
            .ForTeam(team)
            .WithName(name)
            .CreatedAt(createdAt)
            .Build();

        db.Projects.Add(project);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        return await db.Projects
            .AsNoTracking()
            .SingleAsync(x => x.TeamId == team.Id && x.Name == name, TestContext.Current.CancellationToken);
    }
}
