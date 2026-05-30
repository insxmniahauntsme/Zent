using Zent.Data.Entities;

namespace Zent.Unit.Tests.Builders;

public sealed class UserBuilder
{
    private string _email = $"test-{Guid.NewGuid()}@example.com";
    private string _firstName = "Test";
    private string _lastName = "User";
    private string _passwordHash = "password-hash";

    public UserBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public UserBuilder WithName(string firstName, string lastName)
    {
        _firstName = firstName;
        _lastName = lastName;
        return this;
    }

    public UserBuilder WithPasswordHash(string passwordHash)
    {
        _passwordHash = passwordHash;
        return this;
    }

    public UserEntity Build()
        => new()
        {
            Email = _email,
            FirstName = _firstName,
            LastName = _lastName,
            PasswordHash = _passwordHash
        };
}
