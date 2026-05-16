using Microsoft.EntityFrameworkCore;
using Zent.Application.Messaging.Abstractions;
using Zent.Common.Exceptions;
using Zent.Data;

namespace Zent.Application.Features.Projects.GetProjectMembers;

internal sealed class GetProjectMembersHandler(ZentDbContext dbContext)
    : IQueryHandler<GetProjectMembersQuery, IReadOnlyList<ProjectMemberDto>>
{
    public async Task<IReadOnlyList<ProjectMemberDto>> Handle(GetProjectMembersQuery query, CancellationToken ct)
    {
        var project = await dbContext.Projects
            .AsNoTracking()
            .Where(x => x.Id == query.ProjectId)
            .Select(x => new
            {
                x.Id,
                x.TeamId
            })
            .FirstOrDefaultAsync(ct);

        if (project is null)
            throw new ProjectNotFoundException(
                $"Project with id {query.ProjectId} was not found.");

        var hasAccess = await dbContext.ProjectMembers
            .AsNoTracking()
            .AnyAsync(x =>
                    x.ProjectId == project.Id &&
                    x.UserId == query.UserId,
                ct);

        if (!hasAccess)
            throw new ProjectAccessDeniedException(
                "You are not a member of this project.");

        var members = await (
            from projectMember in dbContext.ProjectMembers.AsNoTracking()
            join teamMember in dbContext.TeamMembers.AsNoTracking()
                on projectMember.UserId equals teamMember.UserId
            where projectMember.ProjectId == project.Id
                  && teamMember.TeamId == project.TeamId
            orderby projectMember.User.FirstName,
                projectMember.User.LastName,
                projectMember.User.Email
            select new ProjectMemberDto(
                projectMember.User.Id,
                projectMember.User.FirstName,
                projectMember.User.LastName,
                projectMember.User.Email,
                teamMember.MemberRole,
                teamMember.Specialization)
        ).ToListAsync(ct);

        return members;
    }
}