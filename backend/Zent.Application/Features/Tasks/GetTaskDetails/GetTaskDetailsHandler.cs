using Microsoft.EntityFrameworkCore;
using Zent.Application.Messaging.Abstractions;
using Zent.Common.Exceptions;
using Zent.Data;

namespace Zent.Application.Features.Tasks.GetTaskDetails;

internal sealed class GetTaskDetailsHandler(ZentDbContext dbContext) : IQueryHandler<GetTaskQuery, TaskDetailsDto>
{
    public async Task<TaskDetailsDto> Handle(GetTaskQuery query, CancellationToken ct)
    {
        var task = await dbContext.Tasks
            .AsNoTracking()
            .Where(x => x.Id == query.TaskId)
            .Select(x => new
            {
                x.Id,
                x.Title,
                x.Description,
                x.Priority,
                x.CreatedAt,
                x.UntilDate,
                x.CreatorId,

                x.ColumnId,
                ColumnTitle = x.Column.Title,

                BoardId = x.Column.Board.Id,
                BoardName = x.Column.Board.Name,

                ProjectId = x.Column.Board.Project.Id,
                ProjectName = x.Column.Board.Project.Name,

                x.Column.Board.Project.TeamId,

                Assignee = x.AssigneeId == null
                    ? null
                    : new TaskAssigneeDto(
                        x.Assignee!.Id,
                        x.Assignee.FirstName,
                        x.Assignee.LastName,
                        x.Assignee.Email)
            })
            .FirstOrDefaultAsync(ct);

        if (task is null)
            throw new TaskNotFoundException(
                $"Task with id {query.TaskId} was not found.");

        var hasAccess = await dbContext.TeamMembers
            .AsNoTracking()
            .AnyAsync(x =>
                    x.TeamId == task.TeamId &&
                    x.UserId == query.UserId,
                ct);

        if (!hasAccess)
            throw new TeamAccessDeniedException(
                "You do not have access to this task.");

        return new TaskDetailsDto(
            task.Id,
            task.BoardId,
            task.BoardName,
            task.ProjectId,
            task.ProjectName,
            task.ColumnId,
            task.ColumnTitle,
            task.CreatorId,
            task.Assignee,
            task.Title,
            task.Description,
            task.Priority,
            task.CreatedAt,
            task.UntilDate);
    }
}