using Zent.Application.Messaging.Abstractions;

namespace Zent.Application.Features.Tasks.AddTaskAssignee;

public sealed record AddTaskAssigneeCommand(Guid UserId, Guid BoardId, Guid TaskId, Guid AssigneeId) : ICommand;