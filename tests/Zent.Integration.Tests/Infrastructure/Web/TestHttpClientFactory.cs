using System.Net.Http.Headers;
using System.Net.Http.Json;
using Zent.API.Endpoints.Auth;
using Zent.API.Endpoints.Auth.Login;
using Zent.Application.Features.Auth;
using Zent.Integration.Tests.Infrastructure.Database.Seed;

namespace Zent.Integration.Tests.Infrastructure.Web;

public sealed class TestHttpClientFactory(IntegrationTestWebAppFactory factory)
{
    private const string Uri = "/api/auth/login";

    public HttpClient CreateAnonymous()
        => factory.CreateClient();

    public async Task<HttpClient> CreateAuthenticatedAsync()
    {
        var client = factory.CreateClient();

        var loginResponse =
            await client.PostAsJsonAsync(Uri, new LoginUserRequest(TestUserSeeder.Email, TestUserSeeder.Password));

        loginResponse.EnsureSuccessStatusCode();

        var auth = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", auth?.AccessToken);

        return client;
    }
}