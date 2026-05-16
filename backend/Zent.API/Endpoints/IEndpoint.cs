namespace Zent.API.Endpoints;

public interface IEndpoint
{
    RouteHandlerBuilder Map(IEndpointRouteBuilder app);
}