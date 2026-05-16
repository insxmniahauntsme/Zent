using Microsoft.EntityFrameworkCore;
using Zent.Application.Messaging.Abstractions;
using Zent.Common.Exceptions;
using Zent.Data;

namespace Zent.Application.Features.Users.GetCurrentUser;

internal sealed class GetCurrentUserHandler(ZentDbContext dbContext)
    : IQueryHandler<GetCurrentUserQuery, CurrentUserDto>
{
    public async Task<CurrentUserDto> Handle(GetCurrentUserQuery query, CancellationToken ct)
    {
        var entity = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == query.UserId, ct);

        return entity is null
            ? throw new UserNotFoundException($"User {query.UserId} does not exist.")
            : entity.ToDto();
    }
}