using Microsoft.EntityFrameworkCore;
using Zent.Application.Messaging.Abstractions;
using Zent.Data;

namespace Zent.Application.Features.Users.SearchUsers;

internal sealed class SearchUsersHandler(ZentDbContext dbContext) : IQueryHandler<SearchUsersQuery, List<UserSearchDto>>
{
    public async Task<List<UserSearchDto>> Handle(SearchUsersQuery query, CancellationToken ct)
    {
        var value = query.Query.Trim().ToLower();
        
        return await dbContext.Users
            .AsNoTracking()
            .Where(x =>
                x.Id != query.UserId &&
                (x.Email.ToLower().StartsWith(query.Query) ||
                 x.FirstName.ToLower().Contains(query.Query) ||
                 x.LastName.ToLower().Contains(query.Query)))
            .Select(x => new UserSearchDto(x.Id, x.FirstName, x.LastName, x.Email))
            .Take(query.Limit)
            .ToListAsync(ct);
    }
}