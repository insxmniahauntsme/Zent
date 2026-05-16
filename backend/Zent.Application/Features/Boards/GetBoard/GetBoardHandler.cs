using Microsoft.EntityFrameworkCore;
using Zent.Application.Messaging.Abstractions;
using Zent.Common.Exceptions;
using Zent.Data;

namespace Zent.Application.Features.Boards.GetBoard;

internal sealed class GetBoardHandler(ZentDbContext dbContext)
    : IQueryHandler<GetBoardQuery, BoardDto>
{
    public async Task<BoardDto> Handle(GetBoardQuery query, CancellationToken ct)
    {
        var board = await dbContext.Boards
            .AsNoTracking()
            .Where(x => x.Id == query.BoardId)
            .Select(x => new
            {
                x.Id,
                x.ProjectId,
                x.Name,
                x.Description,
                x.CreatedAt,
                x.UpdatedAt
            })
            .FirstOrDefaultAsync(ct);

        if (board is null)
            throw new BoardNotFoundException(
                $"Board with id {query.BoardId} was not found.");

        var isProjectMember = await dbContext.ProjectMembers
            .AsNoTracking()
            .AnyAsync(x =>
                    x.ProjectId == board.ProjectId &&
                    x.UserId == query.UserId,
                ct);

        if (!isProjectMember)
            throw new ProjectAccessDeniedException(
                "You are not a member of this project.");

        var columns = await dbContext.Columns
            .AsNoTracking()
            .Where(x => x.BoardId == board.Id)
            .OrderBy(x => x.Order)
            .Select(x => new BoardColumnDto(
                x.Id,
                x.Title,
                x.Order,
                x.IsFinal,
                new List<BoardTaskDto>()))
            .ToListAsync(ct);

        var columnIds = columns
            .Select(x => x.Id)
            .ToList();

        var tasks = await dbContext.Tasks
            .AsNoTracking()
            .Where(x => columnIds.Contains(x.ColumnId))
            .OrderBy(x => x.Order)
            .Select(x => new
            {
                x.ColumnId,
                Task = new BoardTaskDto(
                    x.Id,
                    x.CreatorId,
                    x.AssigneeId == null
                        ? null
                        : new BoardTaskAssigneeDto(
                            x.Assignee!.Id,
                            x.Assignee.FirstName,
                            x.Assignee.LastName,
                            x.Assignee.Email),
                    x.Title,
                    x.Description,
                    x.Order,
                    x.Priority,
                    x.UntilDate)
            })
            .ToListAsync(ct);

        var tasksLookup = tasks
            .GroupBy(x => x.ColumnId)
            .ToDictionary(
                x => x.Key,
                x => x.Select(t => t.Task).ToList());

        var resultColumns = columns
            .Select(x => x with
            {
                Tasks = tasksLookup.GetValueOrDefault(x.Id, [])
            })
            .ToList();

        return new BoardDto(
            board.Id,
            board.ProjectId,
            board.Name,
            board.Description,
            board.CreatedAt,
            board.UpdatedAt,
            resultColumns);
    }
}