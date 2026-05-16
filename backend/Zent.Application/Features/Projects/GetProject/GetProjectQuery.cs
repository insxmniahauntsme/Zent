using Zent.Application.Messaging.Abstractions;

namespace Zent.Application.Features.Projects.GetProject;

public sealed record GetProjectQuery(Guid UserId, Guid ProjectId) : IQuery<ProjectDto>;