using Zent.Application.Features.Teams.SearchTeamMembers;

namespace Zent.API.Endpoints.Teams;

internal sealed record TeamMembersResponse(IReadOnlyList<TeamMemberDto> Members);