using Zent.Application.Messaging.Abstractions;

namespace Zent.Application.Features.Columns.DeleteColumn;

public sealed record DeleteColumnCommand(Guid UserId, Guid BoardId, Guid ColumnId) : ICommand;