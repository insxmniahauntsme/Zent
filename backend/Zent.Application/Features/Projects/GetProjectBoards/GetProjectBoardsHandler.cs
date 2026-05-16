using Microsoft.EntityFrameworkCore;
using Zent.Application.Messaging.Abstractions;
using Zent.Common.Exceptions;
using Zent.Data;

namespace Zent.Application.Features.Projects.GetProjectBoards;

internal sealed class GetProjectBoardsHandler(ZentDbContext dbContext)
    : IQueryHandler<GetProjectBoardsQuery, IReadOnlyList<ProjectBoardDto>>
{
    public async Task<IReadOnlyList<ProjectBoardDto>> Handle(GetProjectBoardsQuery query, CancellationToken ct)
    {
        var projectExists = await dbContext.Projects.AnyAsync(x => x.Id == query.ProjectId, ct);
        
        if (!projectExists) throw new ProjectNotFoundException($"Project with id {query.ProjectId} was not found.");
        
        var isProjectMember = await dbContext.ProjectMembers
            .AnyAsync(x => x.ProjectId == query.ProjectId && x.UserId == query.UserId, ct);

        if (!isProjectMember) throw new ProjectAccessDeniedException("You are not a member of this project.");
        
        var boards = await dbContext.Boards
            .AsNoTracking()
            .Where(x => x.ProjectId == query.ProjectId)
            .OrderBy(x => x.CreatedAt)
            .Select(x => new ProjectBoardDto(x.Id, x.Name))
            .ToListAsync(ct);

        return boards;
    }
}