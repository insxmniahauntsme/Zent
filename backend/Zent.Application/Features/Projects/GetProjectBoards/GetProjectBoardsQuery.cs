using Zent.Application.Messaging.Abstractions;

namespace Zent.Application.Features.Projects.GetProjectBoards;

public sealed record GetProjectBoardsQuery(Guid UserId, Guid ProjectId)
    : IQuery<IReadOnlyList<ProjectBoardDto>>;