using Zent.API.Endpoints.Columns.AddColumn;
using Zent.API.Endpoints.Columns.PatchColumn;
using Zent.Application.Features.Columns.AddColumn;
using Zent.Application.Features.Columns.PatchColumn;

namespace Zent.API.Endpoints.Columns;

internal static class ColumnMapper
{
    public static AddColumnCommand ToCommand(this AddColumnRequest request, Guid userId, Guid boardId)
        => new(userId, boardId, request.Title, request.IsFinal);

    public static PatchColumnCommand ToCommand(this PatchColumnRequest request, Guid userId, Guid boardId, Guid columnId)
        => new(userId, boardId, columnId, request.Title, request.IsFinal);
}