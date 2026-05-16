using Microsoft.EntityFrameworkCore;
using Npgsql;
using Zent.Application.Interfaces;
using Zent.Application.Messaging.Abstractions;
using Zent.Common.Exceptions;
using Zent.Data;

namespace Zent.Application.Features.Auth.Register;

internal sealed class RegisterUserHandler(
    ZentDbContext dbContext,
    ITokenService tokenService,
    IPasswordHasher passwordHasher)
    : ICommandHandler<RegisterUserCommand, string>
{
    // TODO: Remove this constant and postgres-related code from the handler.
    private const string UsersEmailIndexName = "IX_users_email";

    public async Task<string> Handle(RegisterUserCommand command, CancellationToken ct)
    {
        if (await dbContext.Users.AnyAsync(x => x.Email == command.Email, ct))
            throw new UserAlreadyExistsException("User already exists.");

        var password = passwordHasher.Hash(command.Password);

        var entity = command.ToEntity(password);
        dbContext.Users.Add(entity);

        try
        {
            await dbContext.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (IsDuplicateEmailViolation(ex))
        {
            throw new UserAlreadyExistsException("User already exists.");
        }

        var token = tokenService.GenerateToken(entity);
        return token;
    }

    private static bool IsDuplicateEmailViolation(DbUpdateException ex)
        => ex.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.UniqueViolation,
            ConstraintName: UsersEmailIndexName
        };
}