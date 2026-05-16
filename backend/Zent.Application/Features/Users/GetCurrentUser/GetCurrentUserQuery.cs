using Zent.Application.Messaging.Abstractions;

namespace Zent.Application.Features.Users.GetCurrentUser;

public sealed record GetCurrentUserQuery(Guid UserId) : IQuery<CurrentUserDto>;