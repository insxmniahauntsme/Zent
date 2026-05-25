using System.Security.Claims;
using Zent.API.Extensions;
using Zent.Application.Features.Teams.GetTeamMembers;
using Zent.Application.Messaging.Abstractions;

namespace Zent.API.Endpoints.Teams.GetTeamMembers;

internal sealed class GetTeamMembersEndpoint : IEndpoint
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapGet("teams/{teamId:guid}/members", Handle)
            .WithName("GetTeamMembers")
            .WithTags("Teams")
            .WithSummary("Gets all members in a team.")
            .RequireAuthorization();
    }

    private static async Task<IResult> Handle(
        ClaimsPrincipal user,
        Guid teamId,
        ICqrsDispatcher dispatcher,
        CancellationToken ct)
    {
        var userId = user.GetRequiredUserId();
        
        var query = new GetTeamMembersQuery(userId, teamId);

        var teamMembers = await dispatcher.Send(query, ct);
        
        return Results.Ok(new TeamMembersResponse(teamMembers));
    }
}