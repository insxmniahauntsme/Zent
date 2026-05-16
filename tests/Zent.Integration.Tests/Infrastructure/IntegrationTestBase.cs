using Xunit;
using Zent.Integration.Tests.Infrastructure.Fixtures;

namespace Zent.Integration.Tests.Infrastructure;

public abstract class IntegrationTestBase(TestHostFixture host): IAsyncLifetime
{
    protected TestHostFixture Host { get; } = host;

    protected IServiceProvider Services => Host.WebAppFactory.Services;

    public ValueTask InitializeAsync() => ValueTask.CompletedTask;
    
    public async ValueTask DisposeAsync()
    {
        await Host.ResetDatabaseAsync();
    }
}