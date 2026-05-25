using Microsoft.EntityFrameworkCore;
using Zent.Application.Messaging.Abstractions;
using Zent.Common.Exceptions;
using Zent.Data;

namespace Zent.Application.Features.Tasks.RemoveTaskAssignee;

internal sealed class RemoveTaskAssigneeHandler(ZentDbContext dbContext)
    : ICommandHandler<RemoveTaskAssigneeCommand>
{
    public async Task Handle(RemoveTaskAssigneeCommand command, CancellationToken ct)
    {
        var board = await dbContext.Boards
            .AsNoTracking()
            .Where(x => x.Id == command.BoardId)
            .Select(x => new
            {
                x.Id,
                x.ProjectId
            })
            .FirstOrDefaultAsync(ct);

        if (board is null)
            throw new BoardNotFoundException(
                $"Board with id {command.BoardId} was not found.");

        var requesterHasAccess = await dbContext.ProjectMembers
            .AsNoTracking()
            .AnyAsync(x =>
                    x.ProjectId == board.ProjectId &&
                    x.UserId == command.UserId,
                ct);

        if (!requesterHasAccess)
            throw new ProjectAccessDeniedException(
                "You are not a member of this project.");

        var task = await dbContext.Tasks
            .Where(x =>
                x.Id == command.TaskId &&
                x.Column.BoardId == board.Id)
            .FirstOrDefaultAsync(ct);

        if (task is null)
            throw new TaskNotFoundException(
                $"Task with id {command.TaskId} was not found in this board.");

        if (task.AssigneeId is null) return;

        task.AssigneeId = null;

        await dbContext.SaveChangesAsync(ct);
    }
}