namespace Zent.API.Endpoints.Columns.AddColumn;

public sealed record AddColumnRequest(bool IsFinal, string? Title = null);