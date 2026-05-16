using Zent.Application.Messaging.Abstractions;

namespace Zent.Application.Features.Users.SearchUsers;

public sealed record SearchUsersQuery(Guid UserId, string Query, int Limit = 10) : IQuery<List<UserSearchDto>>;