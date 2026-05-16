using Zent.Application.Features.Users.GetCurrentUser;
using Zent.Data.Entities;

namespace Zent.Application.Features.Users;

internal static class UserMapper
{
    public static CurrentUserDto ToDto(this UserEntity user)
        => new (user.Id, user.FirstName, user.LastName, user.Email);
}