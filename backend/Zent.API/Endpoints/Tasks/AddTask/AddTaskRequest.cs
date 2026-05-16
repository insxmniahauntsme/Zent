using Zent.Common.Enums;

namespace Zent.API.Endpoints.Tasks.AddTask;

public sealed record AddTaskRequest(
    string Title,
    string? Description,
    TaskPriority Priority,
    DateOnly? UntilDate,
    Guid? AssigneeId);