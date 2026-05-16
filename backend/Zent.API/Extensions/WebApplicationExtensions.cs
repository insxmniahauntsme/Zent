using Zent.API.Endpoints;

namespace Zent.API.Extensions;

public static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        public void MapEndpoints()
        {
            var endpoints = app.Services.GetServices<IEndpoint>();

            foreach (var endpoint in endpoints)
            {
                endpoint.Map(app);
            }
        }
    }
}