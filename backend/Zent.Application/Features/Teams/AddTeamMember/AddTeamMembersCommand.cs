using Zent.Application.Messaging.Abstractions;
using Zent.Common.Enums;

namespace Zent.Application.Features.Teams.AddTeamMember;

public sealed record AddTeamMembersCommand(Guid UserId, Guid TeamId, IReadOnlyList<TeamMemberRoleEntry> Members)
    : ICommand;