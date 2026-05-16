using Zent.Common.Enums;

namespace Zent.Application.Features.Teams.GetUserTeams;

public sealed record UserTeamDto(
    Guid Id,
    Guid OwnerId,
    string Name,
    TeamRole Role,
    int MembersCount,
    int ProjectsCount);