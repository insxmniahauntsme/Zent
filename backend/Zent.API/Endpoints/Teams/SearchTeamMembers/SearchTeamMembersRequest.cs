using Zent.Common.Enums;

namespace Zent.API.Endpoints.Teams.SearchTeamMembers;

public sealed record SearchTeamMembersRequest(string Query, TeamRole? Role, Specialization? Specialization);