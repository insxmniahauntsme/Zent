namespace Zent.Application.Features.Boards.GetBoard;

public sealed record BoardTaskAssigneeDto(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email);