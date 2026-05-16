using Zent.Common.Enums;

namespace Zent.Application.Features.Teams.GetTeam;

public sealed record TeamDto(
    Guid Id,
    Guid OwnerId,
    string Name,
    TeamRole CurrentUserRole, 
    int MembersCount,
    int ProjectsCount);