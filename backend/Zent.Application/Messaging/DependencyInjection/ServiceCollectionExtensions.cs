using Microsoft.Extensions.DependencyInjection;
using Zent.Application.Messaging.Abstractions;

namespace Zent.Application.Messaging.DependencyInjection;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddCqrs()
        {
            var handlerTypes = typeof(ServiceCollectionExtensions).Assembly
                .DefinedTypes
                .Where(t =>
                    t is { IsInterface: false, IsAbstract: false } &&
                    t.ImplementedInterfaces.Any(i =>
                        i.IsGenericType &&
                        (
                            i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                            i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>) ||
                            i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)
                        )));

            foreach (var handlerType in handlerTypes)
            {
                var interfaces = handlerType.ImplementedInterfaces
                    .Where(i =>
                        i.IsGenericType &&
                        (
                            i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                            i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>) ||
                            i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)
                        ));

                foreach (var @interface in interfaces)
                {
                    services.AddScoped(@interface, handlerType);
                }
            }

            services.AddScoped<ICqrsDispatcher, CqrsDispatcher>();

            return services;
        }
    }
}