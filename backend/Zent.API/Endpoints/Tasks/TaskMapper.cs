using Zent.API.Endpoints.Tasks.AddTask;
using Zent.API.Endpoints.Tasks.MoveTask;
using Zent.Application.Features.Tasks.AddTask;
using Zent.Application.Features.Tasks.MoveTask;

namespace Zent.API.Endpoints.Tasks;

internal static class TaskMapper
{
    public static AddTaskCommand ToCommand(this AddTaskRequest request, Guid userId, Guid boardId, Guid columnId)
        => new(userId, boardId, columnId, request.Title, request.Description, request.Priority, request.UntilDate,
            request.AssigneeId);

    public static MoveTaskCommand ToCommand(this MoveTaskRequest request, Guid userId, Guid boardId, Guid taskId)
        => new(userId, boardId, taskId, request.TargetColumnId, request.TargetOrder);
}