using Microsoft.EntityFrameworkCore;
using Zent.Application.Messaging.Abstractions;
using Zent.Common.Enums;
using Zent.Common.Exceptions;
using Zent.Data;
using Zent.Data.Entities;

namespace Zent.Application.Features.Teams.AddTeamMember;
//
// internal sealed class AddTeamMembersHandler(ZentDbContext dbContext) : ICommandHandler<AddTeamMembersCommand>
// {
//     public async Task Handle(AddTeamMembersCommand command, CancellationToken ct)
//     {
//         if (command.Role == TeamRole.Owner)
//             throw new TeamAccessDeniedException("Owner role cannot be assigned this way.");
//         
//         var teamExists = await dbContext.Teams.AnyAsync(x => x.Id == command.TeamId, ct);
//         
//         if (!teamExists)
//             throw new TeamNotFoundException($"Team with id {command.TeamId} was not found.");
//         
//         var hasAccess = await dbContext.TeamMembers
//             .AsNoTracking()
//             .AnyAsync(x =>
//                     x.UserId == command.UserId &&
//                     x.TeamId == command.TeamId &&
//                     x.MemberRole == TeamRole.Owner,
//                 ct);
//
//         if (!hasAccess)
//             throw new TeamAccessDeniedException(
//                 "Only team owner can add members to this team.");
//         
//         var userExists = await dbContext.Users
//             .AsNoTracking()
//             .AnyAsync(x => x.Id == command.MemberId, ct);
//
//         if (!userExists)
//             throw new UserNotFoundException(
//                 $"User with id {command.MemberId} was not found.");
//
//         var alreadyMember = await dbContext.TeamMembers
//             .AsNoTracking()
//             .AnyAsync(x =>
//                     x.TeamId == command.TeamId &&
//                     x.UserId == command.MemberId,
//                 ct);
//
//         if (alreadyMember)
//             throw new TeamMemberAlreadyExistsException(
//                 "User is already a member of this team.");
//
//         var teamMember = new TeamMemberEntity
//         {
//             TeamId = command.TeamId,
//             UserId = command.MemberId,
//             MemberRole = command.Role,
//         };
//
//         dbContext.TeamMembers.Add(teamMember);
//
//         await dbContext.SaveChangesAsync(ct);
//     }
// }