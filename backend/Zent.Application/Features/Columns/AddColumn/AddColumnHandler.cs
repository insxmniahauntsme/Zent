using Microsoft.EntityFrameworkCore;
using Zent.Application.Messaging.Abstractions;
using Zent.Common.Enums;
using Zent.Common.Exceptions;
using Zent.Data;
using Zent.Data.Entities;

namespace Zent.Application.Features.Columns.AddColumn;

internal sealed class AddColumnHandler(ZentDbContext dbContext)
    : ICommandHandler<AddColumnCommand, Guid>
{
    public async Task<Guid> Handle(AddColumnCommand command, CancellationToken ct)
    {
        var board = await dbContext.Boards
            .AsNoTracking()
            .Where(x => x.Id == command.BoardId)
            .Select(x => new
            {
                x.Id,
                x.ProjectId,
                x.Project.TeamId
            })
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

        var title = string.IsNullOrWhiteSpace(command.Title)
            ? await GenerateDefaultColumnTitleAsync(command.BoardId, ct)
            : command.Title.Trim();

        var columnExists = await dbContext.Columns
            .AnyAsync(x =>
                    x.BoardId == board.Id &&
                    x.Title == title,
                ct);

        if (columnExists)
            throw new ColumnAlreadyExistsException(
                "Column with this title already exists.");

        var maxOrder = await dbContext.Columns
            .Where(x => x.BoardId == board.Id)
            .Select(x => (int?)x.Order)
            .MaxAsync(ct) ?? 0;

        var entity = new ColumnEntity
        {
            BoardId = board.Id,
            Title = title,
            Order = maxOrder + 1,
            IsFinal = command.IsFinal
        };

        dbContext.Columns.Add(entity);

        await dbContext.SaveChangesAsync(ct);

        return entity.Id;
    }
    
    private async Task<string> GenerateDefaultColumnTitleAsync(
        Guid boardId,
        CancellationToken cancellationToken)
    {
        const string baseTitle = "Untitled";

        var existingTitles = await dbContext.Columns
            .Where(column => column.BoardId == boardId)
            .Select(column => column.Title)
            .ToListAsync(cancellationToken);

        if (!existingTitles.Contains(baseTitle))
        {
            return baseTitle;
        }

        var index = 2;

        while (existingTitles.Contains($"{baseTitle} {index}"))
        {
            index++;
        }

        return $"{baseTitle} {index}";
    }
}