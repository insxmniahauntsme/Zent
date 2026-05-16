using Microsoft.EntityFrameworkCore;
using Zent.Application.Messaging.Abstractions;
using Zent.Common.Exceptions;
using Zent.Data;

namespace Zent.Application.Features.Projects.GetProject;

internal sealed class GetProjectHandler(ZentDbContext dbContext)
    : IQueryHandler<GetProjectQuery, ProjectDto>
{
    public async Task<ProjectDto> Handle(GetProjectQuery query, CancellationToken ct)
    {
        var project = await dbContext.Projects
            .AsNoTracking()
            .Where(x => x.Id == query.ProjectId)
            .Select(x => new ProjectDto(
                x.Id,
                x.TeamId,
                x.Name,
                x.Description,
                x.Client,
                x.Members.Count,
                x.Boards.Count))
            .FirstOrDefaultAsync(ct);

        if (project is null)
            throw new ProjectNotFoundException($"Project with id {query.ProjectId} was not found.");

        var isProjectMember = await dbContext.ProjectMembers
            .AsNoTracking()
            .AnyAsync(x =>
                    x.UserId == query.UserId &&
                    x.ProjectId == query.ProjectId,
                ct);

        return !isProjectMember
            ? throw new ProjectAccessDeniedException("You are not a member of this project.")
            : project;
    }
}