using Zent.Common.Enums;

namespace Zent.Application.Features.Tasks.GetTaskDetails;

public sealed record TaskDetailsDto(
    Guid Id,
    Guid BoardId,
    string BoardName,
    Guid ProjectId,
    string ProjectName,
    Guid ColumnId,
    string ColumnTitle,
    Guid CreatorId,
    TaskAssigneeDto? Assignee,
    string Title,
    string? Description,
    TaskPriority Priority,
    DateTime CreatedAt,
    DateOnly? UntilDate);