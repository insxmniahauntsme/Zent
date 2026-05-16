using Zent.Application.Features.Teams;

namespace Zent.API.Endpoints.Teams.AddTeam;

public sealed record AddTeamRequest(string Name, List<TeamMemberRoleEntry>? Members);