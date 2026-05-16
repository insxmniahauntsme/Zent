using Microsoft.EntityFrameworkCore;
using Zent.Application.Features.Teams.GetTeam;
using Zent.Application.Messaging.Abstractions;
using Zent.Common.Exceptions;
using Zent.Data;

namespace Zent.Application.Features.Projects.GetTeamProjects;

internal sealed class GetTeamProjectsHandler(ZentDbContext dbContext)
    : IQueryHandler<GetTeamProjectsQuery, IReadOnlyList<TeamProjectDto>>
{
    public async Task<IReadOnlyList<TeamProjectDto>> Handle(
        GetTeamProjectsQuery query,
        CancellationToken ct)
    {
        var team = await dbContext.Teams.FirstOrDefaultAsync(x => x.Id == query.TeamId, ct);
        
        if (team is null) throw new TeamNotFoundException($"Team with id {query.TeamId} was not found.");
        
        var isTeamMember = await dbContext.TeamMembers.AnyAsync(
            x => x.UserId == query.UserId && x.TeamId == query.TeamId,
            ct);

        if (!isTeamMember) throw new TeamAccessDeniedException("You are not a member of this team.");
        
        return await dbContext.Projects
            .AsNoTracking()
            .Where(x => x.TeamId == query.TeamId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new TeamProjectDto(
                x.Id,
                x.Name,
                x.Description,
                x.Client,
                x.Members.Count,
                x.Boards.Count))
            .ToListAsync(ct);
    }
}