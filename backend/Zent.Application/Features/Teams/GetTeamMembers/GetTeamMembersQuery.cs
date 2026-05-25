using Zent.Application.Features.Teams.SearchTeamMembers;
using Zent.Application.Messaging.Abstractions;

namespace Zent.Application.Features.Teams.GetTeamMembers;

public sealed record GetTeamMembersQuery(Guid UserId, Guid TeamId) : IQuery<IReadOnlyList<TeamMemberDto>>;