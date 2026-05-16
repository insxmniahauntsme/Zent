using Zent.Application.Messaging.Abstractions;
using Zent.Common.Enums;

namespace Zent.Application.Features.Tasks.AddTask;

public sealed record AddTaskCommand(
    Guid UserId,
    Guid BoardId,
    Guid ColumnId,
    string Title,
    string? Description,
    TaskPriority Priority,
    DateOnly? UntilDate,
    Guid? AssigneeId)
    : ICommand<Guid>;