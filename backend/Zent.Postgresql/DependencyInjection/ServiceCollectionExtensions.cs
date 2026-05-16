using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Zent.Data;

namespace Zent.Postgresql.DependencyInjection;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddPostgresql(IConfiguration configuration)
        {
            var cs = configuration.GetConnectionString("Zent");

            services.AddDbContext<ZentDbContext>(options =>
                options.UseNpgsql(cs, 
                    x => x.MigrationsAssembly(typeof(ServiceCollectionExtensions).Assembly)));
            
            return services;
        }
    }
}