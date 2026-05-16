using System.Security.Claims;
using FluentValidation;
using Zent.API.Extensions;
using Zent.Application.Messaging.Abstractions;

namespace Zent.API.Endpoints.Tasks.AddTask;

internal sealed class AddTaskEndpoint : IEndpoint
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapPost("/boards/{boardId:guid}/columns/{columnId:guid}/tasks", Handle)
            .WithName("AddTask")
            .WithTags("Tasks")
            .WithSummary("Add a new task")
            .RequireAuthorization();
    }

    private static async Task<IResult> Handle(
        ClaimsPrincipal user,
        Guid boardId,
        Guid columnId,
        AddTaskRequest request,
        IValidator<AddTaskRequest> validator,
        ICqrsDispatcher dispatcher,
        CancellationToken ct)
    {
        await validator.ValidateAndThrowAsync(request, ct);

        var userId = user.GetRequiredUserId();

        var command = request.ToCommand(userId, boardId, columnId);

        var taskId = await dispatcher.Send(command, ct);
        
        //TODO: mb return with CreatedAtRoute
        return Results.Ok(new { taskId });
    }
}