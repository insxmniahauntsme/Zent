using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Zent.Application.Features.Auth;
using Zent.Application.Messaging.Abstractions;

namespace Zent.API.Endpoints.Auth.Login;

internal sealed class LoginEndpoint : IEndpoint
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapPost("/auth/login", Handle)
            .WithName("Login")
            .WithTags("Auth")
            .WithSummary("Login user")
            .AllowAnonymous();
    }

    private static async Task<IResult> Handle(
        [FromBody] LoginUserRequest request,
        [FromServices] ICqrsDispatcher dispatcher,
        [FromServices] IValidator<LoginUserRequest> validator,
        CancellationToken ct)
    {
        await validator.ValidateAndThrowAsync(request, ct);

        var query = request.ToQuery();

        var token = await dispatcher.Send(query, ct);

        return Results.Ok(new AuthResponse(token));
    }
}