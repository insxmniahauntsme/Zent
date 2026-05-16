using Zent.Application.Messaging.Abstractions;

namespace Zent.Application.Features.Columns.PatchColumn;

public sealed record PatchColumnCommand(Guid UserId, Guid BoardId, Guid ColumnId, string? Title, bool? IsFinal)
    : ICommand;