using System.Security.Claims;
using Zent.API.Extensions;
using Zent.Application.Features.Columns.DeleteColumn;
using Zent.Application.Messaging.Abstractions;

namespace Zent.API.Endpoints.Columns.DeleteColumn;

internal sealed class DeleteColumnEndpoint : IEndpoint
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapDelete("/boards/{boardId:guid}/columns/{columnId:guid}", Handle)
            .WithName("DeleteColumn")
            .WithTags("Columns")
            .WithSummary("Delete a column")
            .RequireAuthorization();
    }

    private static async Task<IResult> Handle(
        ClaimsPrincipal user,
        Guid boardId,
        Guid columnId,
        ICqrsDispatcher dispatcher,
        CancellationToken ct)
    {
        var userId = user.GetRequiredUserId();

        var command = new DeleteColumnCommand(userId, boardId, columnId);
        
        await dispatcher.Send(command, ct);
        
        return Results.NoContent();
    }
}