using Microsoft.EntityFrameworkCore;
using Zent.Application.Messaging.Abstractions;
using Zent.Common.Exceptions;
using Zent.Data;

namespace Zent.Application.Features.Teams.SearchTeamMembers;

internal sealed class SearchTeamMembersHandler(ZentDbContext dbContext)
    : IQueryHandler<SearchTeamMembersQuery, IReadOnlyList<TeamMemberDto>>
{
    public async Task<IReadOnlyList<TeamMemberDto>> Handle(
        SearchTeamMembersQuery query,
        CancellationToken ct)
    {
        var value = query.Query.Trim();

        var isTeamMember = await dbContext.TeamMembers.AnyAsync(
            x => x.TeamId == query.TeamId && x.UserId == query.UserId,
            ct);

        if (!isTeamMember)
            throw new TeamAccessDeniedException("You are not a member of this team.");

        return await dbContext.TeamMembers
            .AsNoTracking()
            .Where(x =>
                x.TeamId == query.TeamId &&
                x.UserId != query.UserId &&
                (
                    x.User.Email.StartsWith(value) ||
                    x.User.FirstName.Contains(value) ||
                    x.User.LastName.Contains(value)
                ))
            .Select(x => new TeamMemberDto(
                x.UserId,
                x.User.FirstName,
                x.User.LastName,
                x.User.Email,
                x.MemberRole,
                x.Specialization))
            .Take(query.Limit)
            .ToListAsync(ct);
    }
}