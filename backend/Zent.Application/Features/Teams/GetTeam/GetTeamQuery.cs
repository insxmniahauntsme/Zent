using Zent.Application.Messaging.Abstractions;

namespace Zent.Application.Features.Teams.GetTeam;

public sealed record GetTeamQuery(Guid UserId, Guid TeamId) : IQuery<TeamDto>;