using System.Security.Claims;
using FluentValidation;
using Zent.API.Extensions;
using Zent.Application.Messaging.Abstractions;

namespace Zent.API.Endpoints.Columns.PatchColumn;

internal sealed class PatchColumnEndpoint : IEndpoint
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapPatch("/boards/{boardId:guid}/columns/{columnId:guid}", Handle)
            .WithName("PatchColumn")
            .WithTags("Columns")
            .WithSummary("Patch a column")
            .RequireAuthorization();
    }

    private static async Task<IResult> Handle(
        ClaimsPrincipal user,
        Guid boardId,
        Guid columnId,
        PatchColumnRequest request,
        IValidator<PatchColumnRequest> validator,
        ICqrsDispatcher dispatcher,
        CancellationToken ct)
    {
        await validator.ValidateAndThrowAsync(request, ct);

        var userId = user.GetRequiredUserId();
        
        var command = request.ToCommand(userId, boardId, columnId);
        
        await dispatcher.Send(command, ct);
        
        return Results.NoContent();
    }
}