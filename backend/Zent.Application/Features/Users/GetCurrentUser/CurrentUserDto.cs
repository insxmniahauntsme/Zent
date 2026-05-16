namespace Zent.Application.Features.Users.GetCurrentUser;

public sealed record CurrentUserDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email);