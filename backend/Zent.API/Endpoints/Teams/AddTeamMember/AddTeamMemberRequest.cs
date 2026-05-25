using Zent.Application.Features.Teams;
using Zent.Common.Enums;

namespace Zent.API.Endpoints.Teams.AddTeamMember;

public sealed record AddTeamMemberRequest(IReadOnlyList<TeamMemberRoleEntry> Members);