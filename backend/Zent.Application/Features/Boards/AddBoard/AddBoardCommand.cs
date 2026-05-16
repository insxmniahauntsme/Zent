using Zent.Application.Messaging.Abstractions;

namespace Zent.Application.Features.Boards.AddBoard;

public sealed record AddBoardCommand(Guid UserId, Guid ProjectId, string Name, string? Description) : ICommand<Guid>;