using Microsoft.EntityFrameworkCore;
using Zent.Application.Interfaces;
using Zent.Application.Messaging.Abstractions;
using Zent.Common.Exceptions;
using Zent.Data;

namespace Zent.Application.Features.Auth.Login;

internal sealed class LoginUserHandler(
    ZentDbContext dbContext,
    ITokenService tokenService,
    IPasswordHasher passwordHasher)
    : ICommandHandler<LoginUserCommand, string>
{
    public async Task<string> Handle(LoginUserCommand command, CancellationToken ct)
    {
        var entity = await dbContext.Users
            .FirstOrDefaultAsync(x => x.Email == command.Email, ct);

        if (entity is null)
            throw new InvalidCredentialsException("Invalid credentials.");

        var isValid = passwordHasher.Verify(command.Password, entity.PasswordHash);

        return isValid
            ? tokenService.GenerateToken(entity)
            : throw new InvalidCredentialsException("Invalid credentials.");
    }
}