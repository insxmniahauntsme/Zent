using System.Security.Claims;
using FluentValidation;
using Zent.API.Extensions;
using Zent.Application.Messaging.Abstractions;

namespace Zent.API.Endpoints.Columns.AddColumn;

internal sealed class AddColumnEndpoint : IEndpoint
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapPost("/boards/{boardId:guid}/columns", Handle)
            .WithName("AddColumn")
            .WithTags("Columns")
            .WithSummary("Add a new column")
            .RequireAuthorization();
    }

    private static async Task<IResult> Handle(
        ClaimsPrincipal user,
        Guid boardId,
        AddColumnRequest request,
        IValidator<AddColumnRequest> validator,
        ICqrsDispatcher dispatcher,
        CancellationToken ct)
    {
        await validator.ValidateAndThrowAsync(request, ct);

        var userId = user.GetRequiredUserId();
        
        var command = request.ToCommand(userId, boardId);
        
        var columnId = await dispatcher.Send(command, ct);
        
        // TODO: mb return with CreatedAtRoute
        return Results.Ok(columnId);
    }
}