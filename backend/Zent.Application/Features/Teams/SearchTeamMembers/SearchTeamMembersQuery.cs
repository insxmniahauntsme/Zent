using Zent.Application.Features.Teams.GetTeam;
using Zent.Application.Messaging.Abstractions;
using Zent.Common.Enums;

namespace Zent.Application.Features.Teams.SearchTeamMembers;

public sealed record SearchTeamMembersQuery(
    Guid UserId,
    Guid TeamId,
    string Query,
    TeamRole? Role,
    Specialization? Specialization,
    int Limit = 10) 
    : IQuery<IReadOnlyList<TeamMemberDto>>;