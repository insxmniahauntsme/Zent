using System.Security.Claims;
using FluentValidation;
using Zent.API.Endpoints.Boards.GetBoard;
using Zent.API.Extensions;
using Zent.Application.Messaging.Abstractions;

namespace Zent.API.Endpoints.Boards.AddBoard;

internal sealed class AddBoardEndpoint : IEndpoint
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapPost("/projects/{projectId:guid}/boards", Handle)
            .WithName("AddBoard")
            .WithTags("Boards")
            .WithSummary("Add a new board")
            .RequireAuthorization();   
    }

    private static async Task<IResult> Handle(
        ClaimsPrincipal user,
        Guid projectId,
        AddBoardRequest request,
        IValidator<AddBoardRequest> validator,
        ICqrsDispatcher dispatcher,
        CancellationToken ct)
    {
        await validator.ValidateAndThrowAsync(request, ct);

        var userId = user.GetRequiredUserId();

        var command = request.ToCommand(userId, projectId);
        
        var boardId = await dispatcher.Send(command, ct);

        return Results.CreatedAtRoute(GetBoardEndpoint.Name, new {boardId}, new { id = boardId });
    }
}