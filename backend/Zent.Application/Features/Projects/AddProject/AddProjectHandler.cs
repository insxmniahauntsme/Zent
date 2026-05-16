using Microsoft.EntityFrameworkCore;
using Zent.Application.Messaging.Abstractions;
using Zent.Common.Exceptions;
using Zent.Data;
using Zent.Data.Entities;

namespace Zent.Application.Features.Projects.AddProject;

internal sealed class AddProjectHandler(ZentDbContext dbContext)
    : ICommandHandler<AddProjectCommand, Guid>
{
    public async Task<Guid> Handle(AddProjectCommand command, CancellationToken ct)
    {
        var hasAccess = await dbContext.TeamMembers
            .Where(x => x.TeamId == command.TeamId && x.UserId == command.UserId)
            .WithProjectManageAccess()
            .AnyAsync(ct);

        if (!hasAccess)
            throw new TeamAccessDeniedException(
                "You do not have permission to create projects in this team."
            );

        var name = command.Name.Trim();
        
        var projectExists = await dbContext.Projects
            .AsNoTracking()
            .AnyAsync(x => x.TeamId == command.TeamId && x.Name == name, ct);
        
        if (projectExists) throw new ProjectAlreadyExistsException("Project with this name already exists in this team.");
        
        var entity = command.ToEntity();

        dbContext.Projects.Add(entity);

        var memberIds = (command.Members ?? Enumerable.Empty<Guid>())
            .Append(command.UserId)
            .Distinct()
            .ToList();

        var validMemberIds = await dbContext.TeamMembers
            .Where(x => x.TeamId == command.TeamId && memberIds.Contains(x.UserId))
            .Select(x => x.UserId)
            .ToListAsync(ct);

        var projectMembers = validMemberIds
            .Select(userId => new ProjectMemberEntity
            {
                UserId = userId,
                ProjectId = entity.Id
            });

        dbContext.ProjectMembers.AddRange(projectMembers);

        await dbContext.SaveChangesAsync(ct);

        return entity.Id;
    }
}