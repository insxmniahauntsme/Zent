namespace Zent.Application.Features.Boards.GetBoard;

public sealed record BoardDto(
    Guid Id,
    Guid ProjectId,
    string Name,
    string? Description,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IReadOnlyList<BoardColumnDto> Columns);