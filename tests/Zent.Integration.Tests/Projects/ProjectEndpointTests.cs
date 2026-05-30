using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Zent.API.Endpoints.Projects.AddProject;
using Zent.Common.Enums;
using Zent.Data;
using Zent.Data.Entities;
using Zent.Integration.Tests.Builders;
using Zent.Integration.Tests.Infrastructure;
using Zent.Integration.Tests.Infrastructure.Fixtures;

namespace Zent.Integration.Tests.Projects;

[Collection("Integration")]
public sealed class ProjectEndpointTests(TestHostFixture host)
    : IntegrationTestBase(host)
{
    [Fact]
    public async Task AddProject_ShouldCreateProjectAndMembership_WhenUserCanManageTeam()
    {
        var user = await GetTestUserAsync();
        var team = await SeedTeamAsync(user, TeamRole.Owner);
        var client = await Host.HttpClients.CreateAuthenticatedAsync();
        var request = new AddProjectRequest(
            Name: "Roadmap",
            Description: "Product roadmap",
            Client: "Zent",
            Members: null);

        var response = await client.PostAsJsonAsync(
            $"/api/teams/{team.Id}/projects",
            request,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        await using var stream = await response.Content.ReadAsStreamAsync(TestContext.Current.CancellationToken);
        using var payload = await JsonDocument.ParseAsync(stream, cancellationToken: TestContext.Current.CancellationToken);
        var projectId = payload.RootElement.GetProperty("id").GetGuid();

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ZentDbContext>();

        var project = await db.Projects.SingleAsync(x => x.Id == projectId, TestContext.Current.CancellationToken);
        var membershipExists = await db.ProjectMembers.AnyAsync(
            x => x.ProjectId == projectId && x.UserId == user.Id,
            TestContext.Current.CancellationToken);

        project.TeamId.Should().Be(team.Id);
        project.Name.Should().Be(request.Name);
        membershipExists.Should().BeTrue();
    }

    [Fact]
    public async Task AddProject_ShouldReturnForbidden_WhenUserCannotManageTeam()
    {
        var user = await GetTestUserAsync();
        var team = await SeedTeamAsync(user, TeamRole.Member);
        var client = await Host.HttpClients.CreateAuthenticatedAsync();
        var request = new AddProjectRequest("Roadmap", null, null, null);

        var response = await client.PostAsJsonAsync(
            $"/api/teams/{team.Id}/projects",
            request,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AddProject_ShouldReturnBadRequest_WhenNameIsInvalid()
    {
        var user = await GetTestUserAsync();
        var team = await SeedTeamAsync(user, TeamRole.Owner);
        var client = await Host.HttpClients.CreateAuthenticatedAsync();
        var request = new AddProjectRequest(" ", null, null, null);

        var response = await client.PostAsJsonAsync(
            $"/api/teams/{team.Id}/projects",
            request,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private async Task<TeamEntity> SeedTeamAsync(UserEntity user, TeamRole role)
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

        return team;
    }
}
