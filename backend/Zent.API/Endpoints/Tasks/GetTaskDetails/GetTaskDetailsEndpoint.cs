using System.Security.Claims;
using Zent.API.Extensions;
using Zent.Application.Features.Tasks.GetTaskDetails;
using Zent.Application.Messaging.Abstractions;

namespace Zent.API.Endpoints.Tasks.GetTaskDetails;

internal sealed class GetTaskDetailsEndpoint : IEndpoint
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapGet("/tasks/{taskId:guid}", Handle)
            .WithName("GetTask")
            .WithTags("Tasks")
            .WithSummary("Gets a task by id.")
            .RequireAuthorization();
    }

    private static async Task<IResult> Handle(
        ClaimsPrincipal user,
        Guid taskId,
        ICqrsDispatcher dispatcher,
        CancellationToken ct)
    {
        var userId = user.GetRequiredUserId();
        
        var query = new GetTaskQuery(userId, taskId);

        var task = await dispatcher.Send(query, ct);
        
        return Results.Ok(new TaskDetailsResponse(task));
    }
}