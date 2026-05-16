using Zent.Data.Entities;

namespace Zent.Integration.Tests.Builders;

public sealed class UserBuilder
{
    private string _email = $"test-{Guid.NewGuid()}@example.com";
    private string _password = "test-hash";

    public UserBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public UserBuilder WithPassword(string password)
    {
        _password = password;
        return this;
    }

    public UserEntity Build()
    {
        return new UserEntity
        {
            Email = _email,
            FirstName = "Test",
            LastName = "User",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(_password, BCrypt.Net.BCrypt.GenerateSalt(13))
        };
    }
}