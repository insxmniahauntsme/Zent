using Microsoft.EntityFrameworkCore;
using Zent.Application.Messaging.Abstractions;
using Zent.Common.Exceptions;
using Zent.Data;

namespace Zent.Application.Features.Tasks.AddTaskAssignee;

internal sealed class AddTaskAssigneeHandler(ZentDbContext dbContext)
    : ICommandHandler<AddTaskAssigneeCommand>
{
    public async Task Handle(AddTaskAssigneeCommand command, CancellationToken ct)
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

        var assigneeExists = await dbContext.Users
            .AsNoTracking()
            .AnyAsync(x => x.Id == command.AssigneeId, ct);

        if (!assigneeExists)
            throw new UserNotFoundException(
                $"User with id {command.AssigneeId} was not found.");

        var assigneeIsProjectMember = await dbContext.ProjectMembers
            .AsNoTracking()
            .AnyAsync(x =>
                    x.ProjectId == board.ProjectId &&
                    x.UserId == command.AssigneeId,
                ct);

        if (!assigneeIsProjectMember)
            throw new ProjectAccessDeniedException(
                "Selected assignee is not a member of this project.");

        if (task.AssigneeId == command.AssigneeId) return;

        task.AssigneeId = command.AssigneeId;

        await dbContext.SaveChangesAsync(ct);
    }
}