using Zent.Application.Features.Users.SearchUsers;

namespace Zent.API.Endpoints.Users.SearchUsers;

internal sealed record SearchUsersResponse(List<UserSearchDto> Users);