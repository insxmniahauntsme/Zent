using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Zent.API.Endpoints.Auth;
using Zent.API.Endpoints.Auth.Login;
using Zent.Application.Features.Auth;
using Zent.Data;
using Zent.Integration.Tests.Builders;
using Zent.Integration.Tests.Infrastructure;
using Zent.Integration.Tests.Infrastructure.Fixtures;

namespace Zent.Integration.Tests.Auth;

[Collection("Integration")]
public sealed class LoginEndpointTests(TestHostFixture host) : IntegrationTestBase(host)
{
    private const string Uri = "/api/auth/login";
    
    [Fact]
    public async Task Login_ShouldReturnToken_WhenCredentialsAreValid()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ZentDbContext>();

        var password = "test-password";

        var user = new UserBuilder().WithPassword(password).Build();

        db.Users.Add(user);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var client = Host.HttpClients.CreateAnonymous();

        var request = new LoginUserRequest(
            Email: user.Email,
            Password: password);

        var response = await client.PostAsJsonAsync(Uri, request, TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<AuthResponse>(TestContext.Current.CancellationToken);

        payload.Should().NotBeNull();
        payload!.AccessToken.Should().NotBeNullOrWhiteSpace();
    }
    
    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenPasswordIsInvalid()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ZentDbContext>();

        var user = new UserBuilder().WithPassword("random-password").Build();

        db.Users.Add(user);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var client = Host.HttpClients.CreateAnonymous();

        var request = new LoginUserRequest(user.Email, "wrong-password");

        var response = await client.PostAsJsonAsync(Uri, request, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}