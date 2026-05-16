using Zent.Application.Messaging.Abstractions;

namespace Zent.Application.Features.Boards.GetBoard;

public sealed record GetBoardQuery(Guid UserId, Guid BoardId) : IQuery<BoardDto>;