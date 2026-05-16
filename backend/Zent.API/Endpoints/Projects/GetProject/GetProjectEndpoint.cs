using System.Security.Claims;
using Zent.API.Extensions;
using Zent.Application.Features.Projects.GetProject;
using Zent.Application.Messaging.Abstractions;

namespace Zent.API.Endpoints.Projects.GetProject;

internal sealed class GetProjectEndpoint : IEndpoint
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapGet("/projects/{projectId:guid}", Handle)
            .WithName("GetProject")
            .WithTags("Projects")
            .WithSummary("Gets a project by id.")
            .RequireAuthorization();
    }

    private static async Task<IResult> Handle(
        ClaimsPrincipal user,
        Guid projectId,
        ICqrsDispatcher dispatcher,
        CancellationToken ct)
    {
        var userId = user.GetRequiredUserId();

        var query = new GetProjectQuery(userId, projectId);

        var project = await dispatcher.Send(query, ct);
        
        return Results.Ok(new ProjectResponse(project));
    }
}