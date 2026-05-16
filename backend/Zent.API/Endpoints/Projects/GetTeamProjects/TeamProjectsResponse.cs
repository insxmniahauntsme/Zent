using Zent.Application.Features.Teams.GetTeam;

namespace Zent.API.Endpoints.Projects.GetTeamProjects;

public record TeamProjectsResponse(IReadOnlyList<TeamProjectDto> Projects);