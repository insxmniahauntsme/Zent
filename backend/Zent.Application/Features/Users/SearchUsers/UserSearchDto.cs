namespace Zent.Application.Features.Users.SearchUsers;

public sealed record UserSearchDto(Guid Id, string FirstName, string LastName, string Email);