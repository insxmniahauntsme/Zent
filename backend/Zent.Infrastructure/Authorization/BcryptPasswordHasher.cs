using Zent.Application.Interfaces;

namespace Zent.Infrastructure.Authorization;

public sealed class BcryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password, int workFactor)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor);
    }

    public bool Verify(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}