using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Zent.API.Extensions;
using Zent.Application.Messaging.Abstractions;

namespace Zent.API.Endpoints.Projects.AddProject;

internal sealed class AddProjectEndpoint : IEndpoint
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapPost("teams/{teamId:guid}/projects", Handle)
            .WithName("AddProject")
            .WithTags("Projects")
            .WithSummary("Add a new project")
            .RequireAuthorization();
    }

    private static async Task<IResult> Handle(
        ClaimsPrincipal user,
        Guid teamId,
        [FromBody] AddProjectRequest request,
        ICqrsDispatcher dispatcher,
        IValidator<AddProjectRequest> validator,
        CancellationToken ct)
    {
        await validator.ValidateAndThrowAsync(request, ct);

        var userId = user.GetRequiredUserId();
        
        var command = request.ToCommand(userId, teamId);
        
        var projectId = await dispatcher.Send(command, ct);
        
        return Results.Created($"/teams/{teamId}/projects/{projectId}", new { id = projectId });
    }
}