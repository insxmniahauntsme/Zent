using Zent.API.Endpoints.Boards.AddBoard;
using Zent.Application.Features.Boards.AddBoard;

namespace Zent.API.Endpoints.Boards;

internal static class BoardMapper
{
    public static AddBoardCommand ToCommand(this AddBoardRequest request, Guid userId, Guid projectId)
        => new AddBoardCommand(userId, projectId, request.Name, request.Description);
}