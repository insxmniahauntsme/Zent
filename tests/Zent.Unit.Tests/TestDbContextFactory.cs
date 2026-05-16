using Microsoft.EntityFrameworkCore;
using Zent.Data;

namespace Zent.Unit.Tests;

public static class TestDbContextFactory
{
    public static ZentDbContext Create()
    {
        var options = new DbContextOptionsBuilder<ZentDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ZentDbContext(options);
    }
}