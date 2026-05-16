using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Zent.Data;

namespace Zent.Postgresql;

public sealed class ZentDbContextDesignTimeFactory : IDesignTimeDbContextFactory<ZentDbContext>
{
    // TODO: Use configuration
    public ZentDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ZentDbContext>();

        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=zentdb;Username=postgres;Password=postgres",
            x => x.MigrationsAssembly(typeof(ZentDbContextDesignTimeFactory).Assembly));

        return new ZentDbContext(optionsBuilder.Options);
    }
}