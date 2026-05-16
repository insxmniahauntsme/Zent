using Microsoft.EntityFrameworkCore;
using Zent.Application.Messaging.Abstractions;
using Zent.Common.Enums;
using Zent.Common.Exceptions;
using Zent.Data;
using Zent.Data.Entities;

namespace Zent.Application.Features.Boards.AddBoard;

internal sealed class AddBoardHandler(ZentDbContext dbContext)
    : ICommandHandler<AddBoardCommand, Guid>
{
    public async Task<Guid> Handle(AddBoardCommand command, CancellationToken ct)
    {
        var project = await dbContext.Projects
            .AsNoTracking()
            .Where(x => x.Id == command.ProjectId)
            .Select(x => new { x.Id, x.TeamId })
            .FirstOrDefaultAsync(ct);

        if (project is null)
            throw new ProjectNotFoundException(
                $"Project with id {command.ProjectId} was not found.");

        var hasAccess = await dbContext.TeamMembers
            .AsNoTracking()
            .AnyAsync(x =>
                    x.TeamId == project.TeamId &&
                    x.UserId == command.UserId &&
                    (x.MemberRole == TeamRole.Owner || x.MemberRole == TeamRole.Admin),
                ct);

        if (!hasAccess)
            throw new TeamAccessDeniedException(
                "You do not have permission to create boards in this team.");

        var name = command.Name.Trim();
        
        var boardExists = await dbContext.Boards
            .AsNoTracking()
            .AnyAsync(x => x.ProjectId == project.Id && x.Name == name, ct);
        
        if (boardExists) throw new BoardAlreadyExistsException("Board with this name already exists in this project.");

        var now = DateTime.UtcNow;

        var entity = new BoardEntity
        {
            ProjectId = project.Id,
            Name = name,
            Description = string.IsNullOrWhiteSpace(command.Description)
                ? null
                : command.Description.Trim(),
            CreatedAt = now,
            UpdatedAt = now,
            Columns =
            [
                new ColumnEntity { Title = "Backlog", Order = 1, IsFinal = false },
                new ColumnEntity { Title = "In Progress", Order = 2, IsFinal = false },
                new ColumnEntity { Title = "Review", Order = 3, IsFinal = false },
                new ColumnEntity { Title = "Done", Order = 4, IsFinal = true },
            ]
        };

        dbContext.Boards.Add(entity);

        await dbContext.SaveChangesAsync(ct);

        return entity.Id;
    }
}