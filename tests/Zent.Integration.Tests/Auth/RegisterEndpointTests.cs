using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Zent.API.Endpoints.Auth;
using Zent.API.Endpoints.Auth.Register;
using Zent.Application.Features.Auth;
using Zent.Data;
using Zent.Integration.Tests.Builders;
using Zent.Integration.Tests.Infrastructure;
using Zent.Integration.Tests.Infrastructure.Fixtures;

namespace Zent.Integration.Tests.Auth;

[Collection("Integration")]
public sealed class RegisterEndpointTests(TestHostFixture hostFixture)
    : IntegrationTestBase(hostFixture)
{
    private const string Uri = "/api/auth/register";
    
    [Fact]
    public async Task Register_ShouldReturnToken_WhenRequestIsValid()
    {
        var client = Host.HttpClients.CreateAnonymous();

        var request = new RegisterUserRequest(
            Email: $"test-{Guid.NewGuid()}@example.com",
            FirstName: "Test",
            LastName: "User",
            Password: "password123");
        
        var response = await client.PostAsJsonAsync(Uri, request, TestContext.Current.CancellationToken);
        
        response.EnsureSuccessStatusCode();
        
        var payload = await response.Content.ReadFromJsonAsync<AuthResponse>(TestContext.Current.CancellationToken);
        
        payload.Should().NotBeNull();
        payload.AccessToken.Should().NotBeNullOrEmpty();
    }
    
    [Fact]
    public async Task Register_ShouldReturnConflict_WhenEmailAlreadyExists()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ZentDbContext>();

        var user = new UserBuilder().Build();
        
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        
        var client = Host.HttpClients.CreateAnonymous();
        
        var request = new RegisterUserRequest(
            Email: user.Email,
            FirstName: "Test",
            LastName: "User",
            Password: "password123");
        
        var response = await client.PostAsJsonAsync(Uri, request, TestContext.Current.CancellationToken);
        
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenEmailIsInvalid()
    {
        var user = new UserBuilder().WithEmail("invalid-email").Build();

        var client = Host.HttpClients.CreateAnonymous();
        
        var request = new RegisterUserRequest(
            Email: user.Email,
            FirstName: user.FirstName,
            LastName: user.LastName,
            Password: "password123");
        
        var response = await client.PostAsJsonAsync(Uri, request, TestContext.Current.CancellationToken);
        
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);       
    }
}
