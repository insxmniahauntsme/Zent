using Zent.Application.Messaging.Abstractions;

namespace Zent.Application.Features.Columns.AddColumn;

public sealed record AddColumnCommand(Guid UserId, Guid BoardId, string? Title, bool IsFinal)
    : ICommand<Guid>;