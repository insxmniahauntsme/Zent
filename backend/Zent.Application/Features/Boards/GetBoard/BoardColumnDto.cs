namespace Zent.Application.Features.Boards.GetBoard;

public sealed record BoardColumnDto(
    Guid Id,
    string Title,
    int Order,
    bool IsFinal,
    IReadOnlyList<BoardTaskDto> Tasks);