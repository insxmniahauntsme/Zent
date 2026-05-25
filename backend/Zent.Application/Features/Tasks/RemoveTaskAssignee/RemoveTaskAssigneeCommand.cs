using Zent.Application.Messaging.Abstractions;

namespace Zent.Application.Features.Tasks.RemoveTaskAssignee;

public sealed record RemoveTaskAssigneeCommand(Guid UserId, Guid BoardId, Guid TaskId) : ICommand;