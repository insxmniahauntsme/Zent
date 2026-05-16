namespace Zent.Application.Features.Teams.GetTeam;

public sealed record TeamProjectDto(
    Guid ProjectId,
    string Name,
    string? Description,
    string? Client,
    int MembersCount,
    int BoardsCount);