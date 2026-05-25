using Microsoft.EntityFrameworkCore;
using Zent.Application.Messaging.Abstractions;
using Zent.Common.Exceptions;
using Zent.Data;

namespace Zent.Application.Features.Users.SearchUsers;

internal sealed class SearchUsersHandler(ZentDbContext dbContext)
    : IQueryHandler<SearchUsersQuery, IReadOnlyList<UserSearchDto>>
{
    public async Task<IReadOnlyList<UserSearchDto>> Handle(
        SearchUsersQuery query,
        CancellationToken ct)
    {
        var value = query.Query.Trim().ToLower();

        if (string.IsNullOrWhiteSpace(value))
            return [];

        var hasAccess = await dbContext.TeamMembers
            .AsNoTracking()
            .AnyAsync(x =>
                    x.TeamId == query.TeamId &&
                    x.UserId == query.UserId,
                ct);

        if (!hasAccess)
            throw new TeamAccessDeniedException("You are not a member of this team.");

        var existingTeamMemberIds = dbContext.TeamMembers
            .AsNoTracking()
            .Where(x => x.TeamId == query.TeamId)
            .Select(x => x.UserId);

        return await dbContext.Users
            .AsNoTracking()
            .Where(x =>
                x.Id != query.UserId &&
                !existingTeamMemberIds.Contains(x.Id) &&
                (
                    x.Email.ToLower().StartsWith(value) ||
                    x.FirstName.ToLower().Contains(value) ||
                    x.LastName.ToLower().Contains(value)
                ))
            .OrderBy(x => x.FirstName)
            .ThenBy(x => x.LastName)
            .Select(x => new UserSearchDto(
                x.Id,
                x.FirstName,
                x.LastName,
                x.Email))
            .Take(query.Limit)
            .ToListAsync(ct);
    }
}