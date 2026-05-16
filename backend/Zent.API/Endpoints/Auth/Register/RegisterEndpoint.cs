using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Zent.Application.Features.Auth;
using Zent.Application.Messaging.Abstractions;

namespace Zent.API.Endpoints.Auth.Register;

internal sealed class RegisterEndpoint : IEndpoint
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapPost("/auth/register", Handle)
            .WithName("Register")
            .WithTags("Auth")
            .WithSummary("Register a new user")
            .AllowAnonymous();
    }

    private static async Task<IResult> Handle(
        [FromBody] RegisterUserRequest request,
        [FromServices] ICqrsDispatcher dispatcher,
        [FromServices] IValidator<RegisterUserRequest> validator,
        CancellationToken ct)
    {
        await validator.ValidateAndThrowAsync(request, ct);
        
        var query = request.ToQuery();
        
        var token = await dispatcher.Send(query, ct);

        return Results.Ok(new AuthResponse(token));
    }
}