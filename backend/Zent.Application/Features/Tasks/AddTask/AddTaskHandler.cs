using Microsoft.EntityFrameworkCore;
using Zent.Application.Messaging.Abstractions;
using Zent.Common.Exceptions;
using Zent.Data;
using Zent.Data.Entities;

namespace Zent.Application.Features.Tasks.AddTask;

internal sealed class AddTaskHandler(ZentDbContext dbContext) : ICommandHandler<AddTaskCommand, Guid>
{
    public async Task<Guid> Handle(AddTaskCommand command, CancellationToken ct)
    {
        var board = await dbContext.Boards
            .AsNoTracking()
            .Where(x => x.Id == command.BoardId)
            .Select(x => new { x.Id, x.ProjectId, x.Project.TeamId })
            .FirstOrDefaultAsync(ct);
        
        if (board is null)
            throw new BoardNotFoundException($"Board with id {command.BoardId} was not found.");

        var hasAccess = await dbContext.TeamMembers
            .AsNoTracking()
            .AnyAsync(x =>
                x.TeamId == board.TeamId &&
                x.UserId == command.UserId, ct);

        if (!hasAccess)
            throw new TeamAccessDeniedException(
                "You are not a member of this team.");
        
        var columnExists = await dbContext.Columns
            .AsNoTracking()
            .AnyAsync(x =>
                    x.BoardId == board.Id &&
                    x.Id == command.ColumnId,
                ct);

        if (!columnExists)
            throw new ColumnNotFoundException(
                $"Column with id {command.ColumnId} was not found in this board.");
        
        var title = command.Title.Trim();
        
        var description = string.IsNullOrWhiteSpace(command.Description)
            ? null
            : command.Description.Trim();
        
        var maxOrder = await dbContext.Tasks
            .AsNoTracking()
            .Where(x => x.ColumnId == command.ColumnId)
            .Select(x => (int?)x.Order)
            .MaxAsync(ct) ?? 0;

        var task = new TaskEntity
        {
            ColumnId = command.ColumnId,
            CreatorId = command.UserId,
            AssigneeId = command.AssigneeId,
            Title = title,
            Description = description,
            Priority = command.Priority,
            Order = maxOrder + 1,
            CreatedAt = DateTime.UtcNow,
            UntilDate = command.UntilDate
        };
        
        dbContext.Tasks.Add(task);
        
        await dbContext.SaveChangesAsync(ct);
        
        return task.Id;
    }
}