using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Zent.Data;
using Zent.Integration.Tests.Infrastructure.Database;
using Zent.Integration.Tests.Infrastructure.Database.Seed;
using Zent.Integration.Tests.Infrastructure.Web;

namespace Zent.Integration.Tests.Infrastructure.Fixtures;

public sealed class TestHostFixture : IAsyncLifetime
{
    private static readonly IConfiguration Config =
        new ConfigurationBuilder()
            .AddJsonFile("appsettings.Testing.json")
            .Build();
    
    public IntegrationTestWebAppFactory WebAppFactory { get; private set; } = null!;
    public TestHttpClientFactory HttpClients { get; private set; } = null!;
    private readonly DbCleaner _dbCleaner = new();

    public async ValueTask InitializeAsync()
    {
        var connectionString = Config.GetConnectionString("Zent")!;
        
        await TestDatabaseInitializer.EnsureDatabaseReadyAsync(connectionString);
        
        WebAppFactory = new IntegrationTestWebAppFactory();
        
        using var scope = WebAppFactory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ZentDbContext>();

        await db.Database.EnsureDeletedAsync();
        await db.Database.MigrateAsync();
        
        await new TestUserSeeder().SeedAsync(db);

        await _dbCleaner.InitializeAsync(connectionString);
        
        HttpClients = new TestHttpClientFactory(WebAppFactory);
    }
    
    public async Task ResetDatabaseAsync()
    {
        await _dbCleaner.ResetAsync();

        using var scope = WebAppFactory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ZentDbContext>();

        await new TestUserSeeder().SeedAsync(db);
    }

    public async ValueTask DisposeAsync()
    {
        await WebAppFactory.DisposeAsync();
    }
}
