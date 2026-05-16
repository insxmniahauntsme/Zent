using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Zent.API.Endpoints.Teams.GetTeam;
using Zent.API.Extensions;
using Zent.Application.Messaging.Abstractions;

namespace Zent.API.Endpoints.Teams.AddTeam;

internal sealed class AddTeamEndpoint : IEndpoint
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapPost("/teams", Handle)
            .WithName("AddTeam")
            .WithTags("Teams")
            .WithSummary("Add a new team")
            .RequireAuthorization();
    }

    private static async Task<IResult> Handle(
        ClaimsPrincipal user,
        AddTeamRequest request,
        ICqrsDispatcher dispatcher,
        IValidator<AddTeamRequest> validator,
        CancellationToken ct)
    {
        await validator.ValidateAndThrowAsync(request, ct);

        var command = request.ToCommand(user.GetRequiredUserId());

        var teamId = await dispatcher.Send(command, ct);

        return Results.CreatedAtRoute(GetTeamEndpoint.Name, new { teamId }, new { id = teamId });
    }
}