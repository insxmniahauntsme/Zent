using Microsoft.EntityFrameworkCore;
using Zent.Application.Messaging.Abstractions;
using Zent.Data;

namespace Zent.Application.Features.Teams.GetUserTeams;

internal sealed class GetUserTeamsHandler(ZentDbContext dbContext) : IQueryHandler<GetUserTeamsQuery, List<UserTeamDto>>
{
    public async Task<List<UserTeamDto>> Handle(GetUserTeamsQuery query, CancellationToken ct)
    {
        return await dbContext.TeamMembers
            .AsNoTracking()
            .Where(x => x.UserId == query.UserId)
            .Select(x => new UserTeamDto(
                x.Team.Id,
                x.Team.OwnerId,
                x.Team.Name,
                x.MemberRole,
                x.Team.TeamMemberships.Count,
                x.Team.Projects.Count))
            .ToListAsync(ct);
    }
}