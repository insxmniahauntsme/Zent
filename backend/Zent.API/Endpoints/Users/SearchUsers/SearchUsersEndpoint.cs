using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Zent.API.Extensions;
using Zent.Application.Features.Users.SearchUsers;
using Zent.Application.Messaging.Abstractions;

namespace Zent.API.Endpoints.Users.SearchUsers;

internal sealed class SearchUsersEndpoint : IEndpoint
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapGet("/teams/{teamId:guid}/users", Handle)
            .WithName("SearchUser")
            .WithTags("Users")
            .WithSummary("Search for users by name or email.")
            .RequireAuthorization();
    }

    private static async Task<IResult> Handle(
        ClaimsPrincipal user,
        Guid teamId,
        [AsParameters] SearchUsersRequest request,
        ICqrsDispatcher dispatcher,
        IValidator<SearchUsersRequest> validator,
        CancellationToken ct)
    {
        await validator.ValidateAndThrowAsync(request, ct);
        
        var userId = user.GetRequiredUserId();
        
        var query = new SearchUsersQuery(userId, teamId, request.Query);
        
        var data = await dispatcher.Send(query, ct);

        return Results.Ok(new SearchUsersResponse(data));
    }
}