namespace Zent.Application.Interfaces;

public interface IPasswordHasher
{
    string Hash(string password, int workFactor = 13);
    bool Verify(string password, string hashedPassword);
}