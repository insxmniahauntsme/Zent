using Zent.Application.Features.Projects.AddProject;
using Zent.Data.Entities;

namespace Zent.Application.Features.Projects;

internal static class ProjectMapper
{
    public static ProjectEntity ToEntity(this AddProjectCommand command)
        => new()
        {
            TeamId = command.TeamId, 
            Name = command.Name.Trim(), 
            Description = command.Description, 
            Client = command.Client,
            CreatedAt = DateTime.UtcNow
        };
}