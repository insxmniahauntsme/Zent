using Microsoft.EntityFrameworkCore;
using Zent.Application.Messaging.Abstractions;
using Zent.Common.Enums;
using Zent.Common.Exceptions;
using Zent.Data;

namespace Zent.Application.Features.Teams.GetTeam;

internal sealed class GetTeamHandler(ZentDbContext dbContext) : IQueryHandler<GetTeamQuery, TeamDto>
{
    public async Task<TeamDto> Handle(GetTeamQuery query, CancellationToken ct)
    {
        var currentUserRole = await dbContext.TeamMembers
            .AsNoTracking()
            .Where(x => x.TeamId == query.TeamId && x.UserId == query.UserId)
            .Select(x => (TeamRole?)x.MemberRole)
            .FirstOrDefaultAsync(ct);

        if (currentUserRole is null)
            throw new TeamNotFoundException("Team was not found.");

        var team = await dbContext.Teams
            .AsNoTracking()
            .Where(x => x.Id == query.TeamId)
            .Select(x => new TeamDto(
                x.Id,
                x.OwnerId,
                x.Name,
                currentUserRole.Value,
                x.TeamMemberships.Count,
                x.Projects.Count
            ))
            .FirstOrDefaultAsync(ct);

        return team ?? throw new TeamNotFoundException("Team was not found.");
    }
}