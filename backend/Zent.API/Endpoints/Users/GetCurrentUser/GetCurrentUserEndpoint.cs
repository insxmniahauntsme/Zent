using System.Security.Claims;
using Zent.API.Extensions;
using Zent.Application.Features.Users.GetCurrentUser;
using Zent.Application.Messaging.Abstractions;

namespace Zent.API.Endpoints.Users.GetCurrentUser;

internal sealed class GetCurrentUserEndpoint : IEndpoint
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapGet("/users/me", Handle)
            .WithName("GetCurrentUser")
            .WithTags("Users")
            .WithSummary("Gets current user.")
            .RequireAuthorization();
    }

    private static async Task<IResult> Handle(
        ClaimsPrincipal user,
        ICqrsDispatcher dispatcher,
        CancellationToken ct)
    {
        var userId = user.GetRequiredUserId();

        var request = new GetCurrentUserQuery(userId);

        var currentUser = await dispatcher.Send(request, ct);
        
        return Results.Ok(currentUser);
    }
}