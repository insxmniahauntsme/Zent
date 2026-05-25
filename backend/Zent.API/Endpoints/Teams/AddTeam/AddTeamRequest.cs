using Zent.Application.Features.Teams;
using Zent.Application.Features.Teams.AddTeam;

namespace Zent.API.Endpoints.Teams.AddTeam;

public sealed record AddTeamRequest(string Name, List<TeamMemberRoleEntry>? Members);