using Microsoft.EntityFrameworkCore;
using Zent.Application.Messaging.Abstractions;
using Zent.Common.Exceptions;
using Zent.Data;

namespace Zent.Application.Features.Columns.PatchColumn;

internal sealed class PatchColumnHandler(ZentDbContext dbContext)
    : ICommandHandler<PatchColumnCommand>
{
    public async Task Handle(PatchColumnCommand command, CancellationToken ct)
    {
        var column = await dbContext.Columns
            .Include(x => x.Board)
            .ThenInclude(x => x.Project)
            .FirstOrDefaultAsync(x =>
                    x.Id == command.ColumnId &&
                    x.BoardId == command.BoardId,
                ct);

        if (column is null)
            throw new ColumnNotFoundException(
                $"Column with id {command.ColumnId} was not found.");

        var hasAccess = await dbContext.TeamMembers
            .Where(x =>
                x.TeamId == column.Board.Project.TeamId &&
                x.UserId == command.UserId)
            .WithBoardManageAccess()
            .AnyAsync(ct);

        if (!hasAccess)
            throw new TeamAccessDeniedException(
                "You do not have permission to manage columns in this team.");

        if (command.Title is not null)
        {
            var title = command.Title.Trim();

            var exists = await dbContext.Columns.AnyAsync(x =>
                    x.BoardId == command.BoardId &&
                    x.Id != command.ColumnId &&
                    x.Title == title,
                ct);

            if (exists)
                throw new ColumnAlreadyExistsException(
                    "Column with this title already exists.");

            column.Title = title;
        }

        if (command.IsFinal is not null)
        {
            column.IsFinal = command.IsFinal.Value;
        }

        await dbContext.SaveChangesAsync(ct);
    }
}