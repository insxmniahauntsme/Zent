using Microsoft.Extensions.DependencyInjection;
using Zent.Application.Interfaces;
using Zent.Infrastructure.Authorization;

namespace Zent.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure()
        {
            services.AddSingleton<ITokenService, JwtTokenService>();
            services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
            
            return services;
        }
    }
}