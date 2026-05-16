namespace Zent.API.Endpoints.Tasks.MoveTask;

public sealed record MoveTaskRequest(Guid TargetColumnId, int TargetOrder);