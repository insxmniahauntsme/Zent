using System.Net;
using System.Text.Json;
using FluentAssertions;
using Xunit;
using Zent.Integration.Tests.Infrastructure;
using Zent.Integration.Tests.Infrastructure.Fixtures;

namespace Zent.Integration.Tests.Users;

[Collection("Integration")]
public sealed class GetCurrentUserEndpointTests(TestHostFixture host)
    : IntegrationTestBase(host)
{
    private const string Uri = "/api/users/me";

    [Fact]
    public async Task GetCurrentUser_ShouldReturnCurrentUser_WhenAuthenticated()
    {
        var user = await GetTestUserAsync();
        var client = await Host.HttpClients.CreateAuthenticatedAsync();

        var response = await client.GetAsync(Uri, TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(TestContext.Current.CancellationToken);
        using var payload = await JsonDocument.ParseAsync(stream, cancellationToken: TestContext.Current.CancellationToken);

        payload.RootElement.GetProperty("id").GetGuid().Should().Be(user.Id);
        payload.RootElement.GetProperty("email").GetString().Should().Be(user.Email);
        payload.RootElement.GetProperty("firstName").GetString().Should().Be(user.FirstName);
        payload.RootElement.GetProperty("lastName").GetString().Should().Be(user.LastName);
    }

    [Fact]
    public async Task GetCurrentUser_ShouldReturnUnauthorized_WhenAnonymous()
    {
        var client = Host.HttpClients.CreateAnonymous();

        var response = await client.GetAsync(Uri, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
