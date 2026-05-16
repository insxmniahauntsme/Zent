using Microsoft.EntityFrameworkCore;
using Zent.Application.Messaging.Abstractions;
using Zent.Common.Enums;
using Zent.Common.Exceptions;
using Zent.Data;

namespace Zent.Application.Features.Columns.DeleteColumn;

internal sealed class DeleteColumnHandler(ZentDbContext dbContext) : ICommandHandler<DeleteColumnCommand>
{
    public async Task Handle(DeleteColumnCommand command, CancellationToken ct)
    {
        var board = await dbContext.Boards
            .AsNoTracking()
            .Where(x => x.Id == command.BoardId)
            .Select(x => new { x.Id, x.ProjectId, x.Project.TeamId })
            .FirstOrDefaultAsync(ct);
        
        if (board is null)
            throw new BoardNotFoundException($"Board with id {command.BoardId} was not found.");

        var hasAccess = await dbContext.TeamMembers.AnyAsync(x =>
                x.TeamId == board.TeamId &&
                x.UserId == command.UserId &&
                (x.MemberRole == TeamRole.Owner || x.MemberRole == TeamRole.Admin),
            ct);

        if (!hasAccess)
            throw new TeamAccessDeniedException(
                "You do not have permission to manage columns in this team.");
        
        var column = await dbContext.Columns
            .FirstOrDefaultAsync(x =>
                    x.Id == command.ColumnId &&
                    x.BoardId == command.BoardId,
                ct);

        if (column is null) throw new ColumnNotFoundException($"Column with id {command.ColumnId} was not found.");
        
        dbContext.Columns.Remove(column);

        var remainingColumns = await dbContext.Columns
            .Where(x =>
                x.BoardId == command.BoardId &&
                x.Id != command.ColumnId)
            .OrderBy(x => x.Order)
            .ToListAsync(ct);

        for (var index = 0; index < remainingColumns.Count; index++)
        {
            remainingColumns[index].Order = index + 1;
        }

        await dbContext.SaveChangesAsync(ct);
    }
}