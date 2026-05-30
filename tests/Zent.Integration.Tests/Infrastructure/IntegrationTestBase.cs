using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Zent.Data;
using Zent.Data.Entities;
using Zent.Integration.Tests.Infrastructure.Database.Seed;
using Zent.Integration.Tests.Infrastructure.Fixtures;

namespace Zent.Integration.Tests.Infrastructure;

public abstract class IntegrationTestBase(TestHostFixture host): IAsyncLifetime
{
    protected TestHostFixture Host { get; } = host;

    protected IServiceProvider Services => Host.WebAppFactory.Services;

    protected async Task<UserEntity> GetTestUserAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ZentDbContext>();

        return await db.Users
            .AsNoTracking()
            .SingleAsync(x => x.Email == TestUserSeeder.Email, TestContext.Current.CancellationToken);
    }

    public ValueTask InitializeAsync() => ValueTask.CompletedTask;

    public async ValueTask DisposeAsync()
    {
        await Host.ResetDatabaseAsync();
    }
}
