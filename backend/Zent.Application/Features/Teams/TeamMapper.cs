using Zent.Application.Features.Teams.AddTeam;
using Zent.Data.Entities;

namespace Zent.Application.Features.Teams;

internal static class TeamMapper
{
    public static TeamEntity ToEntity(this AddTeamCommand command)
        => new() { Name = command.Name, OwnerId = command.CreatorId};
}