using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zent.Data;
using Zent.Integration.Tests.Infrastructure.Database.Seed;

namespace Zent.Integration.Tests.Infrastructure.Web;

public sealed class IntegrationTestWebAppFactory : WebApplicationFactory<Program>
{
    private readonly TestUserSeeder _seeder = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices((context, services) =>
        {
            var connectionString = context.Configuration.GetConnectionString("Zent");

            services.RemoveAll<DbContextOptions<ZentDbContext>>();

            services.AddDbContext<ZentDbContext>(options => { options.UseNpgsql(connectionString); });
        });
    }
}