using System.Security.Claims;
using Zent.API.Extensions;
using Zent.Application.Features.Tasks.AddTaskAssignee;
using Zent.Application.Messaging.Abstractions;

namespace Zent.API.Endpoints.Tasks.AddTaskAssignee;

internal sealed class AddTaskAssigneeEndpoint : IEndpoint
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapPatch("/boards/{boardId:guid}/tasks/{taskId:guid}/assignee", Handle)
            .WithName("AddTaskAssignee")
            .WithTags("Tasks")
            .WithSummary("Adds or replaces task assignee.")
            .RequireAuthorization();
    }

    private static async Task<IResult> Handle(
        ClaimsPrincipal user,
        Guid boardId,
        Guid taskId,
        AddTaskAssigneeRequest request,
        ICqrsDispatcher dispatcher,
        CancellationToken ct)
    {
        var userId = user.GetRequiredUserId();
        
        var command = new AddTaskAssigneeCommand(userId, boardId, taskId, request.AssigneeId);
        
        await dispatcher.Send(command, ct);
        
        return Results.NoContent();
    }
}