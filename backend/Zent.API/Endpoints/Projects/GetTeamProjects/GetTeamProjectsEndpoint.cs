using System.Security.Claims;
using Zent.API.Extensions;
using Zent.Application.Features.Projects.GetTeamProjects;
using Zent.Application.Messaging.Abstractions;

namespace Zent.API.Endpoints.Projects.GetTeamProjects;

internal sealed class GetTeamProjectsEndpoint : IEndpoint
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapGet("teams/{teamId:guid}/projects", Handle)
            .WithName("GetTeamProjects")
            .WithTags("Projects")
            .WithSummary("Gets all projects in a team.")
            .RequireAuthorization();
    }

    private static async Task<IResult> Handle(
        ClaimsPrincipal user,
        Guid teamId,
        ICqrsDispatcher dispatcher,
        CancellationToken ct)
    {
        var userId = user.GetRequiredUserId();

        var query = new GetTeamProjectsQuery(userId, teamId);

        var projects = await dispatcher.Send(query, ct);

        return Results.Ok(new TeamProjectsResponse(projects));
    }
}