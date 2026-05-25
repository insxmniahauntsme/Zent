using System.Security.Claims;
using Zent.API.Extensions;
using Zent.Application.Features.Tasks.RemoveTaskAssignee;
using Zent.Application.Messaging.Abstractions;

namespace Zent.API.Endpoints.Tasks.RemoveTaskAssignee;

internal sealed class RemoveTaskAssigneeEndpoint : IEndpoint
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapDelete("/boards/{boardId:guid}/tasks/{taskId:guid}/assignee/remove", Handle)
            .WithName("RemoveTaskAssignee")
            .WithTags("Tasks")
            .WithSummary("Removes a project member from a task.")
            .RequireAuthorization();
    }
    
    private static async Task<IResult> Handle(
        ClaimsPrincipal user,
        Guid boardId,
        Guid taskId,
        ICqrsDispatcher dispatcher,
        CancellationToken ct)
    {
        var userId = user.GetRequiredUserId();
        
        var command = new RemoveTaskAssigneeCommand(userId, boardId, taskId);

        await dispatcher.Send(command, ct);
        
        return Results.NoContent();
    }
}