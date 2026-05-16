using System.Security.Claims;
using Zent.API.Extensions;
using Zent.Application.Features.Boards.GetBoard;
using Zent.Application.Messaging.Abstractions;

namespace Zent.API.Endpoints.Boards.GetBoard;

internal sealed class GetBoardEndpoint : IEndpoint
{
    public const string Name = "GetBoard";
    
    public RouteHandlerBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapGet("/boards/{boardId:guid}", Handle)
            .WithName(Name)
            .WithTags("Boards")
            .WithSummary("Gets a board by id.")
            .RequireAuthorization();
    }

    private static async Task<IResult> Handle(
        ClaimsPrincipal user,
        Guid boardId,
        ICqrsDispatcher dispatcher,
        CancellationToken ct)
    {
        var userId = user.GetRequiredUserId();
        
        var query = new GetBoardQuery(userId, boardId);

        var board = await dispatcher.Send(query, ct);
        
        return Results.Ok(new BoardResponse(board));
    }
}