using Zent.Application.Features.Teams.GetTeam;
using Zent.Application.Messaging.Abstractions;

namespace Zent.Application.Features.Projects.GetTeamProjects;

public sealed record GetTeamProjectsQuery(Guid UserId, Guid TeamId) : IQuery<IReadOnlyList<TeamProjectDto>>;