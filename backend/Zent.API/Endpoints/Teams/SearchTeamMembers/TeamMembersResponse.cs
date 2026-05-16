using Zent.Application.Features.Teams.GetTeam;
using Zent.Application.Features.Teams.SearchTeamMembers;

namespace Zent.API.Endpoints.Teams.SearchTeamMembers;

internal sealed record TeamMembersResponse(IReadOnlyList<TeamMemberDto> Members);