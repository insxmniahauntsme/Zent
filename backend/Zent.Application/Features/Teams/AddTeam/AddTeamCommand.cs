using Zent.Application.Messaging.Abstractions;

namespace Zent.Application.Features.Teams.AddTeam;

//TODO: MemberIds should be a dictionary with user id as key and role as value
public sealed record AddTeamCommand(Guid CreatorId, string Name, List<TeamMemberRoleEntry>? Members) : ICommand<Guid>;