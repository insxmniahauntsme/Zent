using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.OpenApi;
using Zent.API.Endpoints;

namespace Zent.API.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddEndpoints(Assembly assembly)
        {
            var types = assembly
                .DefinedTypes
                .Where(x => typeof(IEndpoint).IsAssignableFrom(x) && x is { IsInterface: false, IsAbstract: false });

            foreach (var type in types)
            {
                services.AddTransient(typeof(IEndpoint), type);
            }

            return services;
        }

        public IServiceCollection AddOpenApiWithJwtAuth()
        {
            services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, ct) =>
                {
                    document.Components ??= new OpenApiComponents();
                    document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                    document.Security ??= [];

                    document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT"
                    };

                    document.Security.Add(new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                    });

                    return Task.CompletedTask;
                });
            });
            
            return services;
        }

        public IServiceCollection AddJsonStringEnumConversion()
        {
            return services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
        }
    }
}
