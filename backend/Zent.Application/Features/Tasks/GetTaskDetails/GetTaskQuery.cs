using Zent.Application.Messaging.Abstractions;

namespace Zent.Application.Features.Tasks.GetTaskDetails;

public sealed record GetTaskQuery(Guid UserId, Guid TaskId) : IQuery<TaskDetailsDto>;