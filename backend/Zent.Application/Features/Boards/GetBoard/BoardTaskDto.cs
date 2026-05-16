using Zent.Common.Enums;

namespace Zent.Application.Features.Boards.GetBoard;

public sealed record BoardTaskDto(
    Guid Id,
    Guid CreatorId,
    BoardTaskAssigneeDto? Assignee,
    string Title,
    string? Description,
    int Order,
    TaskPriority Priority,
    DateOnly? UntilDate);