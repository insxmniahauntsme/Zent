using System.Security.Claims;
using FluentValidation;
using Zent.API.Extensions;
using Zent.Application.Features.Tasks.MoveTask;
using Zent.Application.Messaging.Abstractions;

namespace Zent.API.Endpoints.Tasks.MoveTask;

internal sealed class MoveTaskEndpoint : IEndpoint
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapPatch("/boards/{boardId:guid}/tasks/{taskId:guid}/move", Handle)
            .WithName("MoveTask")
            .WithTags("Tasks")
            .WithSummary("Moves a task")
            .RequireAuthorization();
    }
    
    private static async Task<IResult> Handle(
        ClaimsPrincipal user,
        Guid boardId,
        Guid taskId,
        MoveTaskRequest request,
        IValidator<MoveTaskRequest> validator,
        ICqrsDispatcher dispatcher,
        CancellationToken ct)
    {
        await validator.ValidateAndThrowAsync(request, ct);

        var userId = user.GetRequiredUserId();
        
        var command = new MoveTaskCommand(userId, boardId, taskId, request.TargetColumnId, request.TargetOrder);

        await dispatcher.Send(command, ct);
        
        return Results.NoContent();
    }
}