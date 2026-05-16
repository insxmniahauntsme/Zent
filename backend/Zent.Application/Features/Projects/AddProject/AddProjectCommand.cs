using Zent.Application.Messaging.Abstractions;

namespace Zent.Application.Features.Projects.AddProject;

public record AddProjectCommand(
    Guid UserId,
    Guid TeamId,
    string Name,
    string? Description,
    string? Client,
    List<Guid>? Members)
    : ICommand<Guid>;