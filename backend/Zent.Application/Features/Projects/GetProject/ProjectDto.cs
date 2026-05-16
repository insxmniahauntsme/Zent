namespace Zent.Application.Features.Projects.GetProject;

public record ProjectDto(
    Guid Id,
    Guid TeamId,
    string Name,
    string? Description,
    string? Client,
    int MembersCount,
    int BoardsCount);