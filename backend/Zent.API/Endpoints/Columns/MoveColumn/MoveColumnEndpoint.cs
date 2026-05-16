using System.Security.Claims;
using FluentValidation;
using Zent.API.Extensions;
using Zent.Application.Features.Columns.MoveColumn;
using Zent.Application.Messaging.Abstractions;

namespace Zent.API.Endpoints.Columns.MoveColumn;

internal sealed class MoveColumnEndpoint : IEndpoint
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapPatch("/boards/{boardId:guid}/columns/{columnId:guid}/move", Handle)
            .WithName("MoveColumn")
            .WithTags("Columns")
            .WithSummary("Moves a column")
            .RequireAuthorization();
    }

    private static async Task<IResult> Handle(
        ClaimsPrincipal user,
        Guid boardId,
        Guid columnId,
        MoveColumnRequest request,
        IValidator<MoveColumnRequest> validator,
        ICqrsDispatcher dispatcher,
        CancellationToken ct)
    {
        await validator.ValidateAndThrowAsync(request, ct);

        var userId = user.GetRequiredUserId();

        var command = new MoveColumnCommand(userId, boardId, columnId, request.TargetOrder);
        
        await dispatcher.Send(command, ct);
        
        return Results.NoContent();
    }
}