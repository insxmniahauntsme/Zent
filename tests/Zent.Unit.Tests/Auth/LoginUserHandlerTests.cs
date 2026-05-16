using FluentAssertions;
using Moq;
using Xunit;
using Zent.Application.Features.Auth;
using Zent.Application.Features.Auth.Login;
using Zent.Application.Interfaces;
using Zent.Common.Exceptions;
using Zent.Data;
using Zent.Data.Entities;

namespace Zent.Unit.Tests.Auth;

public class LoginUserHandlerTests
{
    private readonly Mock<ITokenService> _tokenService = new();
    private readonly Mock<IPasswordHasher> _hasher = new();

    private LoginUserHandler CreateHandler(ZentDbContext db)
        => new LoginUserHandler(db, _tokenService.Object, _hasher.Object);

    [Fact]
    public async Task Should_ReturnToken_When_CredentialsAreValid()
    {
        var db = TestDbContextFactory.Create();

        var user = new UserEntity
        {
            Email = "test@test.com",
            PasswordHash = "hashed",
            FirstName = "Test",
            LastName = "User"
        };

        db.Users.Add(user);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        _hasher
            .Setup(x => x.Verify("123456", "hashed"))
            .Returns(true);

        _tokenService
            .Setup(x => x.GenerateToken(user))
            .Returns("token");

        var handler = CreateHandler(db);

        var query = new LoginUserCommand("test@test.com", "123456");

        var result = await handler.Handle(query, TestContext.Current.CancellationToken);

        result.Should().Be("token");

        _hasher.Verify(x =>
            x.Verify("123456", "hashed"),
            Times.Once);

        _tokenService.Verify(x =>
            x.GenerateToken(user),
            Times.Once);
    }

    [Fact]
    public async Task Should_Throw_When_UserNotFound()
    {
        var db = TestDbContextFactory.Create();

        var handler = CreateHandler(db);

        var query = new LoginUserCommand("notfound@test.com", "123456");

        var act = () => handler.Handle(query, TestContext.Current.CancellationToken);

        await act.Should()
            .ThrowAsync<InvalidCredentialsException>();

        _hasher.VerifyNoOtherCalls();
        _tokenService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Should_Throw_When_PasswordInvalid()
    {
        var db = TestDbContextFactory.Create();

        var user = new UserEntity
        {
            Email = "test@test.com",
            PasswordHash = "hashed",
            FirstName = "Test",
            LastName = "User"
        };

        db.Users.Add(user);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        _hasher
            .Setup(x => x.Verify("wrong-password", "hashed"))
            .Returns(false);

        var handler = CreateHandler(db);

        var query = new LoginUserCommand("test@test.com", "wrong-password");

        var act = () => handler.Handle(query, TestContext.Current.CancellationToken);

        await act.Should()
            .ThrowAsync<InvalidCredentialsException>();

        _tokenService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Should_CallPasswordHasher()
    {
        var db = TestDbContextFactory.Create();

        var user = new UserEntity
        {
            Email = "test@test.com",
            PasswordHash = "hashed",
            FirstName = "Test",
            LastName = "User"
        };

        db.Users.Add(user);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        _hasher
            .Setup(x => x.Verify(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);

        _tokenService
            .Setup(x => x.GenerateToken(It.IsAny<UserEntity>()))
            .Returns("token");

        var handler = CreateHandler(db);

        await handler.Handle(new LoginUserCommand("test@test.com", "123456"), TestContext.Current.CancellationToken);

        _hasher.Verify(x =>
            x.Verify("123456", "hashed"),
            Times.Once);
    }
}