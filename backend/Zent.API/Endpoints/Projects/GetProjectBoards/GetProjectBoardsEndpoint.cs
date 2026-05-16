using System.Security.Claims;
using Zent.API.Extensions;
using Zent.Application.Features.Projects.GetProjectBoards;
using Zent.Application.Messaging.Abstractions;

namespace Zent.API.Endpoints.Projects.GetProjectBoards;

internal sealed class GetProjectBoardsEndpoint : IEndpoint
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapGet("/projects/{projectId:guid}/boards", Handle)
            .WithName("GetProjectBoards")
            .WithTags("Projects")
            .WithSummary("Gets all boards in a project.")
            .RequireAuthorization();
    }

    private static async Task<IResult> Handle(
        ClaimsPrincipal user,
        Guid projectId,
        ICqrsDispatcher dispatcher,
        CancellationToken ct)
    {
        var userId = user.GetRequiredUserId();
        
        var query = new GetProjectBoardsQuery(userId, projectId);

        var boards = await dispatcher.Send(query, ct);

        return Results.Ok(new ProjectBoardsResponse(boards));
    }
}