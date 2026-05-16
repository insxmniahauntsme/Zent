using Zent.API.Endpoints.Teams.AddTeam;
using Zent.API.Endpoints.Teams.SearchTeamMembers;
using Zent.Application.Features.Teams.AddTeam;
using Zent.Application.Features.Teams.SearchTeamMembers;

namespace Zent.API.Endpoints.Teams;

internal static class TeamMapper
{
    public static AddTeamCommand ToCommand(this AddTeamRequest request, Guid creatorId)
        => new(creatorId, request.Name, request.Members);

    public static SearchTeamMembersQuery ToQuery(this SearchTeamMembersRequest request, Guid userId, Guid teamId)
        => new(userId, teamId, request.Query, request.Role, request.Specialization);
}