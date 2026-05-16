using Zent.Application.Messaging.Abstractions;

namespace Zent.Application.Features.Teams.GetUserTeams;

public sealed record GetUserTeamsQuery(Guid UserId) : IQuery<List<UserTeamDto>>;