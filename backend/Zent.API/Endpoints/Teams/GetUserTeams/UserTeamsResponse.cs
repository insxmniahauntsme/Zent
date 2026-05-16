using Zent.Application.Features.Teams;
using Zent.Application.Features.Teams.GetUserTeams;

namespace Zent.API.Endpoints.Teams.GetUserTeams;

internal sealed record UserTeamsResponse(List<UserTeamDto> Teams);