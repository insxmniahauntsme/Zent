using Zent.Application.Messaging.Abstractions;

namespace Zent.Application.Features.Tasks.MoveTask;

public sealed record MoveTaskCommand(Guid UserId, Guid BoardId, Guid TaskId, Guid TargetColumnId, int TargetOrder)
    : ICommand;