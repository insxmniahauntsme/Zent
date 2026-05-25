using Zent.Application.Messaging.Abstractions;

namespace Zent.Application.Features.Users.SearchUsers;

public sealed record SearchUsersQuery(Guid UserId, Guid TeamId, string Query, int Limit = 10) : IQuery<List<UserSearchDto>>;