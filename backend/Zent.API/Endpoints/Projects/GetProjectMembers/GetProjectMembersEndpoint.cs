using System.Security.Claims;
using Zent.API.Extensions;
using Zent.Application.Features.Projects.GetProjectMembers;
using Zent.Application.Messaging.Abstractions;

namespace Zent.API.Endpoints.Projects.GetProjectMembers;

internal sealed class GetProjectMembersEndpoint : IEndpoint
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapGet("/projects/{projectId:guid}/members", Handle)
            .WithName("GetProjectMembers")
            .WithTags("Projects")
            .WithSummary("Gets all members in a project.")
            .RequireAuthorization();
    }

    private static async Task<IResult> Handle(
        ClaimsPrincipal user,
        Guid projectId,
        ICqrsDispatcher dispatcher,
        CancellationToken ct)
    {
        var userId = user.GetRequiredUserId();

        var query = new GetProjectMembersQuery(userId, projectId);

        var members = await dispatcher.Send(query, ct);
        
        return Results.Ok(new ProjectMembersResponse(members));
    }
}