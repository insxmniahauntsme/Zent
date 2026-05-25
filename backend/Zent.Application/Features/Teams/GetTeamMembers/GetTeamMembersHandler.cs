using Microsoft.EntityFrameworkCore;
using Zent.Application.Features.Teams.SearchTeamMembers;
using Zent.Application.Messaging.Abstractions;
using Zent.Common.Exceptions;
using Zent.Data;

namespace Zent.Application.Features.Teams.GetTeamMembers;

internal sealed class GetTeamMembersHandler(ZentDbContext dbContext)
    : IQueryHandler<GetTeamMembersQuery, IReadOnlyList<TeamMemberDto>>
{
    public async Task<IReadOnlyList<TeamMemberDto>> Handle(GetTeamMembersQuery query, CancellationToken ct)
    {
        var hasAccess =
            await dbContext.TeamMembers.AnyAsync(x => x.UserId == query.UserId && x.TeamId == query.TeamId, ct);
        
        if (!hasAccess)
            throw new TeamAccessDeniedException("You are not a member of this team.");
        
        var members = await dbContext.TeamMembers
            .AsNoTracking()
            .Where(x => x.TeamId == query.TeamId)
            .OrderBy(x => x.User.FirstName)
            .ThenBy(x => x.User.LastName)
            .Select(x => new TeamMemberDto(
                x.UserId,
                x.User.FirstName,
                x.User.LastName,
                x.User.Email,
                x.MemberRole,
                x.Specialization))
            .ToListAsync(ct);

        return members;
    }
}