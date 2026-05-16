using Zent.API.Endpoints.Projects.AddProject;
using Zent.Application.Features.Projects.AddProject;

namespace Zent.API.Endpoints.Projects;

internal static class ProjectMapper
{
    public static AddProjectCommand ToCommand(this AddProjectRequest request, Guid userId, Guid teamId)
        => new(userId, teamId, request.Name, request.Description, request.Client, request.Members);
}