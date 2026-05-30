using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Zent.API.Endpoints.Teams.AddTeam;
using Zent.Common.Enums;
using Zent.Data;
using Zent.Integration.Tests.Builders;
using Zent.Integration.Tests.Infrastructure;
using Zent.Integration.Tests.Infrastructure.Fixtures;

namespace Zent.Integration.Tests.Teams;

[Collection("Integration")]
public sealed class TeamEndpointTests(TestHostFixture host)
    : IntegrationTestBase(host)
{
    [Fact]
    public async Task AddTeam_ShouldCreateTeamAndOwnerMembership_WhenRequestIsValid()
    {
        var user = await GetTestUserAsync();
        var client = await Host.HttpClients.CreateAuthenticatedAsync();
        var request = new AddTeamRequest($"Team {Guid.NewGuid():N}"[..12], null);

        var response = await client.PostAsJsonAsync("/api/teams", request, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        await using var stream = await response.Content.ReadAsStreamAsync(TestContext.Current.CancellationToken);
        using var payload = await JsonDocument.ParseAsync(stream, cancellationToken: TestContext.Current.CancellationToken);
        var teamId = payload.RootElement.GetProperty("id").GetGuid();

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ZentDbContext>();

        var team = await db.Teams.SingleAsync(x => x.Id == teamId, TestContext.Current.CancellationToken);
        var membership = await db.TeamMembers.SingleAsync(
            x => x.TeamId == teamId && x.UserId == user.Id,
            TestContext.Current.CancellationToken);

        team.Name.Should().Be(request.Name);
        team.OwnerId.Should().Be(user.Id);
        membership.MemberRole.Should().Be(TeamRole.Owner);
    }

    [Fact]
    public async Task GetUserTeams_ShouldReturnOnlyTeamsForCurrentUser()
    {
        var user = await GetTestUserAsync();
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ZentDbContext>();

        var team = new TeamBuilder().WithOwner(user).WithName("Alpha Team").Build();
        var otherUser = new UserBuilder().WithEmail($"other-{Guid.NewGuid()}@example.com").Build();

        db.Users.Add(otherUser);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        otherUser = await db.Users
            .AsNoTracking()
            .SingleAsync(x => x.Email == otherUser.Email, TestContext.Current.CancellationToken);

        var otherTeam = new TeamBuilder().WithOwner(otherUser).WithName("Other Team").Build();

        db.Teams.AddRange(team, otherTeam);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        team = await db.Teams
            .AsNoTracking()
            .SingleAsync(
                x => x.OwnerId == user.Id && x.Name == "Alpha Team",
                TestContext.Current.CancellationToken);
        otherTeam = await db.Teams
            .AsNoTracking()
            .SingleAsync(
                x => x.OwnerId == otherUser.Id && x.Name == "Other Team",
                TestContext.Current.CancellationToken);

        db.TeamMembers.AddRange(
            new TeamMemberBuilder().ForUser(user).ForTeam(team).WithRole(TeamRole.Owner).Build(),
            new TeamMemberBuilder().ForUser(otherUser).ForTeam(otherTeam).WithRole(TeamRole.Owner).Build());
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var client = await Host.HttpClients.CreateAuthenticatedAsync();

        var response = await client.GetAsync("/api/teams/my", TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(TestContext.Current.CancellationToken);
        using var payload = await JsonDocument.ParseAsync(stream, cancellationToken: TestContext.Current.CancellationToken);
        var teams = payload.RootElement.GetProperty("teams").EnumerateArray().ToList();

        teams.Should().ContainSingle(x => x.GetProperty("id").GetGuid() == team.Id);
        teams.Should().NotContain(x => x.GetProperty("id").GetGuid() == otherTeam.Id);
    }

    [Fact]
    public async Task GetTeam_ShouldReturnNotFound_WhenCurrentUserIsNotTeamMember()
    {
        var otherUser = new UserBuilder().WithEmail($"other-{Guid.NewGuid()}@example.com").Build();

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ZentDbContext>();
        db.Users.Add(otherUser);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        otherUser = await db.Users
            .AsNoTracking()
            .SingleAsync(x => x.Email == otherUser.Email, TestContext.Current.CancellationToken);

        var team = new TeamBuilder().WithOwner(otherUser).Build();

        db.Teams.Add(team);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        team = await db.Teams
            .AsNoTracking()
            .SingleAsync(
                x => x.OwnerId == otherUser.Id && x.Name == team.Name,
                TestContext.Current.CancellationToken);

        db.TeamMembers.Add(new TeamMemberBuilder().ForUser(otherUser).ForTeam(team).WithRole(TeamRole.Owner).Build());
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var client = await Host.HttpClients.CreateAuthenticatedAsync();

        var response = await client.GetAsync($"/api/teams/{team.Id}", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
