using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Zent.API.Extensions;
using Zent.Application.Features.Teams.GetUserTeams;
using Zent.Application.Messaging.Abstractions;

namespace Zent.API.Endpoints.Teams.GetUserTeams;

internal sealed class GetUserTeamsEndpoint : IEndpoint
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapGet("/teams/my", Handle)
            .WithName("Get user teams")
            .WithTags("Teams")
            .WithSummary("Gets current user teams.")
            .RequireAuthorization();
    }

    private static async Task<IResult> Handle(
        ClaimsPrincipal user,
        [FromServices] ICqrsDispatcher dispatcher,
        CancellationToken ct)
    {
        var userId = user.GetRequiredUserId();
        
        var query = new GetUserTeamsQuery(userId);
        
        var data = await dispatcher.Send(query, ct);

        return Results.Ok(new UserTeamsResponse(data));
    }
}