using Zent.Application.Messaging.Abstractions;

namespace Zent.Application.Features.Columns.MoveColumn;

public sealed record MoveColumnCommand(Guid UserId, Guid BoardId, Guid ColumnId, int TargetOrder) : ICommand;