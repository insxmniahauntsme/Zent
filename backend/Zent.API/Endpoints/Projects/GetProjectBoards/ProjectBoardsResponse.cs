using Zent.Application.Features.Projects.GetProjectBoards;

namespace Zent.API.Endpoints.Projects.GetProjectBoards;

public sealed record ProjectBoardsResponse(IReadOnlyList<ProjectBoardDto> Boards);