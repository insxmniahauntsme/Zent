using Zent.Data.Entities;

namespace Zent.Application.Features.Auth.Register;

internal static class RegisterUserMapper
{
    internal static UserEntity ToEntity(this RegisterUserCommand command, string password)
        => new()
        {
            Email = command.Email,
            FirstName = command.FirstName,
            LastName = command.LastName,
            PasswordHash = password
        };
}