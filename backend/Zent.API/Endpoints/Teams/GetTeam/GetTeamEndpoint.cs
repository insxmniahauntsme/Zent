using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Zent.API.Extensions;
using Zent.Application.Features.Teams.GetTeam;
using Zent.Application.Messaging.Abstractions;

namespace Zent.API.Endpoints.Teams.GetTeam;

internal sealed class GetTeamEndpoint : IEndpoint
{
    public const string Name = "GetTeam";
    
    public RouteHandlerBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapGet("/teams/{teamId:guid}", Handle)
            .WithName(Name)
            .WithTags("Teams")
            .WithSummary("Gets a team by id.")
            .RequireAuthorization();
    }

    private static async Task<IResult> Handle(
        ClaimsPrincipal user,
        Guid teamId,
        ICqrsDispatcher dispatcher,
        CancellationToken ct)
    {
        var query = new GetTeamQuery(user.GetRequiredUserId(), teamId);

        var team = await dispatcher.Send(query, ct);
        
        return Results.Ok(new TeamResponse(team));
    }
}