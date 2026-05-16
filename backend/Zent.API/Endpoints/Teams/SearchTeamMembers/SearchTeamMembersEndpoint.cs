using System.Security.Claims;
using Zent.API.Extensions;
using Zent.Application.Messaging.Abstractions;

namespace Zent.API.Endpoints.Teams.SearchTeamMembers;

internal sealed class SearchTeamMembersEndpoint : IEndpoint
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapGet("teams/{teamId:guid}/members/search", Handle)
            .WithName("SearchTeamMembers")
            .WithTags("Teams")
            .WithSummary("Search for team members by name or email.")
            .RequireAuthorization();
    }

    private static async Task<IResult> Handle(
        ClaimsPrincipal user,
        Guid teamId,
        [AsParameters] SearchTeamMembersRequest request,
        ICqrsDispatcher dispatcher,
        CancellationToken ct)
    {
        var userId = user.GetRequiredUserId();
        
        var query = request.ToQuery(userId, teamId);

        var users = await dispatcher.Send(query, ct);

        return Results.Ok(new TeamMembersResponse(users));
    }
}