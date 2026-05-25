using System.Security.Claims;
using Zent.API.Extensions;
using Zent.Application.Features.Teams.AddTeamMember;
using Zent.Application.Messaging.Abstractions;

namespace Zent.API.Endpoints.Teams.AddTeamMember;

internal sealed class AddTeamMemberEndpoint : IEndpoint
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapPost("teams/{teamId:guid}/members", Handle)
            .WithName("AddTeamMember")
            .WithTags("Teams")
            .WithSummary("Add a new member to a team.")
            .RequireAuthorization();
    }

    private static async Task<IResult> Handle(
        ClaimsPrincipal user,
        Guid teamId,
        AddTeamMemberRequest request,
        ICqrsDispatcher dispatcher,
        CancellationToken ct)
    {
        var userId = user.GetRequiredUserId();

        var command = new AddTeamMembersCommand(userId, teamId, request.Members);
        
        await dispatcher.Send(command, ct);
        
        return Results.NoContent();
    }
}