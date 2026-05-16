using Microsoft.EntityFrameworkCore;
using Zent.Application.Messaging.Abstractions;
using Zent.Common.Exceptions;
using Zent.Data;

namespace Zent.Application.Features.Columns.MoveColumn;

internal sealed class MoveColumnHandler(ZentDbContext dbContext) : ICommandHandler<MoveColumnCommand>
{
    public async Task Handle(MoveColumnCommand command, CancellationToken ct)
    {
        var board = await dbContext.Boards
            .AsNoTracking()
            .Where(x => x.Id == command.BoardId)
            .Select(x => new { x.Id, x.ProjectId })
            .FirstOrDefaultAsync(ct);

        if (board is null) throw new BoardNotFoundException($"Board with id {command.BoardId} was not found.");

        var hasAccess = await dbContext.ProjectMembers
            .AsNoTracking()
            .AnyAsync(x =>
                    x.ProjectId == board.ProjectId &&
                    x.UserId == command.UserId,
                ct);

        if (!hasAccess) throw new ProjectAccessDeniedException("You are not a member of this project.");

        var columns = await dbContext.Columns
            .Where(x => x.BoardId == command.BoardId)
            .OrderBy(x => x.Order)
            .ToListAsync(ct);

        var movingColumn = columns.FirstOrDefault(x => x.Id == command.ColumnId);

        if (movingColumn is null)
            throw new ColumnNotFoundException($"Column with id {command.ColumnId} was not found.");
        
        var targetOrder = Math.Clamp(command.TargetOrder, 1, columns.Count);
        
        columns.Remove(movingColumn);
        columns.Insert(targetOrder - 1, movingColumn);

        for (var index = 0; index < columns.Count; index++)
            columns[index].Order = index + 1;

        await dbContext.SaveChangesAsync(ct);
    }
}