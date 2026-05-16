using Microsoft.EntityFrameworkCore;
using Zent.Application.Messaging.Abstractions;
using Zent.Common.Exceptions;
using Zent.Data;

namespace Zent.Application.Features.Tasks.MoveTask;

internal sealed class MoveTaskHandler(ZentDbContext dbContext)
    : ICommandHandler<MoveTaskCommand>
{
    public async Task Handle(MoveTaskCommand command, CancellationToken ct)
    {
        var board = await dbContext.Boards
            .AsNoTracking()
            .Where(x => x.Id == command.BoardId)
            .Select(x => new { x.Id, x.ProjectId })
            .FirstOrDefaultAsync(ct);

        if (board is null)
        {
            throw new BoardNotFoundException(
                $"Board with id {command.BoardId} was not found.");
        }

        var hasAccess = await dbContext.ProjectMembers
            .AsNoTracking()
            .AnyAsync(x =>
                    x.ProjectId == board.ProjectId &&
                    x.UserId == command.UserId,
                ct);

        if (!hasAccess)
            throw new ProjectAccessDeniedException(
                "You are not a member of this project.");

        var task = await dbContext.Tasks
            .FirstOrDefaultAsync(x => x.Id == command.TaskId, ct);

        if (task is null)
            throw new TaskNotFoundException(
                $"Task with id {command.TaskId} was not found.");

        var taskBelongsToBoard = await dbContext.Columns
            .AsNoTracking()
            .AnyAsync(x =>
                    x.Id == task.ColumnId &&
                    x.BoardId == command.BoardId,
                ct);

        if (!taskBelongsToBoard)
            throw new TaskNotFoundException(
                $"Task with id {command.TaskId} does not belong to this board.");

        var targetColumnExists = await dbContext.Columns
            .AsNoTracking()
            .AnyAsync(x =>
                    x.Id == command.TargetColumnId &&
                    x.BoardId == command.BoardId,
                ct);

        if (!targetColumnExists)
            throw new ColumnNotFoundException(
                $"Column with id {command.TargetColumnId} was not found in this board.");

        var sourceColumnId = task.ColumnId;
        var targetColumnId = command.TargetColumnId;

        if (sourceColumnId == targetColumnId)
        {
            await MoveInsideSameColumnAsync(
                taskId: task.Id,
                columnId: sourceColumnId,
                targetOrder: command.TargetOrder,
                ct);
        }
        else
        {
            await MoveBetweenColumnsAsync(
                taskId: task.Id,
                sourceColumnId: sourceColumnId,
                targetColumnId: targetColumnId,
                targetOrder: command.TargetOrder,
                ct);
        }

        await dbContext.SaveChangesAsync(ct);
    }

    private async Task MoveInsideSameColumnAsync(
        Guid taskId,
        Guid columnId,
        int targetOrder,
        CancellationToken ct)
    {
        var tasks = await dbContext.Tasks
            .Where(x => x.ColumnId == columnId)
            .OrderBy(x => x.Order)
            .ToListAsync(ct);

        var movingTask = tasks.FirstOrDefault(x => x.Id == taskId);

        if (movingTask is null)
            throw new TaskNotFoundException(
                $"Task with id {taskId} was not found in this column.");

        tasks.Remove(movingTask);

        var insertIndex = Math.Clamp(targetOrder - 1, 0, tasks.Count);

        tasks.Insert(insertIndex, movingTask);

        for (var i = 0; i < tasks.Count; i++)
            tasks[i].Order = i + 1;
    }

    private async Task MoveBetweenColumnsAsync(
        Guid taskId,
        Guid sourceColumnId,
        Guid targetColumnId,
        int targetOrder,
        CancellationToken ct)
    {
        var sourceTasks = await dbContext.Tasks
            .Where(x => x.ColumnId == sourceColumnId)
            .OrderBy(x => x.Order)
            .ToListAsync(ct);

        var targetTasks = await dbContext.Tasks
            .Where(x => x.ColumnId == targetColumnId)
            .OrderBy(x => x.Order)
            .ToListAsync(ct);

        var movingTask = sourceTasks.FirstOrDefault(x => x.Id == taskId);

        if (movingTask is null)
            throw new TaskNotFoundException(
                $"Task with id {taskId} was not found in source column.");

        sourceTasks.Remove(movingTask);

        var insertIndex = Math.Clamp(targetOrder - 1, 0, targetTasks.Count);

        movingTask.ColumnId = targetColumnId;

        targetTasks.Insert(insertIndex, movingTask);

        for (var i = 0; i < sourceTasks.Count; i++)
            sourceTasks[i].Order = i + 1;

        for (var i = 0; i < targetTasks.Count; i++)
            targetTasks[i].Order = i + 1;
    }
}