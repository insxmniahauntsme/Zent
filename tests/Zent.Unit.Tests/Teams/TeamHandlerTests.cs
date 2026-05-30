using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Zent.Application.Features.Teams.AddTeam;
using Zent.Application.Features.Teams.GetTeam;
using Zent.Application.Features.Teams.GetUserTeams;
using Zent.Common.Enums;
using Zent.Common.Exceptions;
using Zent.Data;
using Zent.Data.Entities;
using Zent.Unit.Tests.Builders;

namespace Zent.Unit.Tests.Teams;

public sealed class TeamHandlerTests
{
    [Fact]
    public async Task AddTeam_ShouldCreateTeamAndOwnerMembership()
    {
        await using var db = TestDbContextFactory.Create();
        var user = await SeedUserAsync(db);
        var handler = new AddTeamHandler(db);
        var command = new AddTeamCommand(user.Id, "Core Team", null);

        var teamId = await handler.Handle(command, TestContext.Current.CancellationToken);

        var team = await db.Teams.SingleAsync(x => x.Id == teamId, TestContext.Current.CancellationToken);
        var membership = await db.TeamMembers.SingleAsync(
            x => x.TeamId == teamId && x.UserId == user.Id,
            TestContext.Current.CancellationToken);

        team.Name.Should().Be("Core Team");
        team.OwnerId.Should().Be(user.Id);
        membership.MemberRole.Should().Be(TeamRole.Owner);
    }

    [Fact]
    public async Task GetUserTeams_ShouldReturnOnlyCurrentUserTeams()
    {
        await using var db = TestDbContextFactory.Create();
        var user = await SeedUserAsync(db);
        var otherUser = await SeedUserAsync(db, $"other-{Guid.NewGuid()}@example.com");
        var team = await SeedTeamAsync(db, user, TeamRole.Owner, "Alpha Team");
        var otherTeam = await SeedTeamAsync(db, otherUser, TeamRole.Owner, "Other Team");
        var handler = new GetUserTeamsHandler(db);

        var result = await handler.Handle(
            new GetUserTeamsQuery(user.Id),
            TestContext.Current.CancellationToken);

        result.Should().ContainSingle(x => x.Id == team.Id);
        result.Should().NotContain(x => x.Id == otherTeam.Id);
    }

    [Fact]
    public async Task GetTeam_ShouldReturnTeam_WhenUserIsMember()
    {
        await using var db = TestDbContextFactory.Create();
        var user = await SeedUserAsync(db);
        var team = await SeedTeamAsync(db, user, TeamRole.Admin, "Alpha Team");
        var handler = new GetTeamHandler(db);

        var result = await handler.Handle(
            new GetTeamQuery(user.Id, team.Id),
            TestContext.Current.CancellationToken);

        result.Id.Should().Be(team.Id);
        result.Name.Should().Be(team.Name);
        result.CurrentUserRole.Should().Be(TeamRole.Admin);
    }

    [Fact]
    public async Task GetTeam_ShouldThrow_WhenUserIsNotMember()
    {
        await using var db = TestDbContextFactory.Create();
        var user = await SeedUserAsync(db);
        var otherUser = await SeedUserAsync(db, $"other-{Guid.NewGuid()}@example.com");
        var team = await SeedTeamAsync(db, otherUser, TeamRole.Owner, "Other Team");
        var handler = new GetTeamHandler(db);

        var act = () => handler.Handle(
            new GetTeamQuery(user.Id, team.Id),
            TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<TeamNotFoundException>();
    }

    private static async Task<UserEntity> SeedUserAsync(
        ZentDbContext db,
        string? email = null)
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
        TeamRole role,
        string name)
    {
        var team = new TeamBuilder().WithOwner(user).WithName(name).Build();

        db.Teams.Add(team);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        team = await db.Teams
            .AsNoTracking()
            .SingleAsync(x => x.OwnerId == user.Id && x.Name == name, TestContext.Current.CancellationToken);

        db.TeamMembers.Add(new TeamMemberBuilder().ForUser(user).ForTeam(team).WithRole(role).Build());
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        return team;
    }
}
