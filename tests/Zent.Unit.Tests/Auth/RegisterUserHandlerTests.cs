using FluentAssertions;
using Moq;
using Xunit;
using Zent.Application.Features.Auth;
using Zent.Application.Features.Auth.Register;
using Zent.Application.Interfaces;
using Zent.Common.Exceptions;
using Zent.Data;
using Zent.Data.Entities;

namespace Zent.Unit.Tests.Auth;

public class RegisterUserHandlerTests
{
    private readonly Mock<ITokenService> _tokenService = new();
    private readonly Mock<IPasswordHasher> _hasher = new();

    private RegisterUserHandler CreateHandler(ZentDbContext db)
        => new RegisterUserHandler(db, _tokenService.Object, _hasher.Object);

    [Fact]
    public async Task Should_CreateUser_AndReturnToken()
    {
        var db = TestDbContextFactory.Create();

        _hasher
            .Setup(x => x.Hash("123456", It.IsAny<int>()))
            .Returns("hashed");

        _tokenService
            .Setup(x => x.GenerateToken(It.IsAny<UserEntity>()))
            .Returns("token");

        var handler = CreateHandler(db);

        var query = new RegisterUserCommand(
            Email: "test@test.com",
            Password: "123456",
            FirstName: "Test",
            LastName: "User");

        var result = await handler.Handle(query, TestContext.Current.CancellationToken);

        result.Should().Be("token");

        db.Users.Should().ContainSingle(x =>
            x.Email == "test@test.com" &&
            x.PasswordHash == "hashed");

        _hasher.Verify(x =>
            x.Hash("123456", It.IsAny<int>()),
            Times.Once);

        _tokenService.Verify(x =>
            x.GenerateToken(It.Is<UserEntity>(u => u.Email == "test@test.com")),
            Times.Once);
    }

    [Fact]
    public async Task Should_Throw_When_EmailAlreadyExists()
    {
        var db = TestDbContextFactory.Create();

        db.Users.Add(new UserEntity
        {
            Email = "test@test.com",
            PasswordHash = "hash",
            FirstName = "Test",
            LastName = "User"
        });

        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = CreateHandler(db);

        var query = new RegisterUserCommand(
            Email: "test@test.com",
            Password: "123456",
            FirstName: "Test",
            LastName: "User");

        var act = () => handler.Handle(query, TestContext.Current.CancellationToken);

        await act.Should()
            .ThrowAsync<UserAlreadyExistsException>();
    }

    [Fact]
    public async Task Should_CallPasswordHasher()
    {
        var db = TestDbContextFactory.Create();

        _hasher
            .Setup(x => x.Hash(It.IsAny<string>(), It.IsAny<int>()))
            .Returns("hashed");

        _tokenService
            .Setup(x => x.GenerateToken(It.IsAny<UserEntity>()))
            .Returns("token");

        var handler = CreateHandler(db);

        var query = new RegisterUserCommand(
            Email: "test@test.com",
            Password: "123456",
            FirstName: "Test",
            LastName: "User");

        await handler.Handle(query, TestContext.Current.CancellationToken);

        _hasher.Verify(x =>
            x.Hash("123456", It.IsAny<int>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_CallTokenService_WithCreatedUser()
    {
        var db = TestDbContextFactory.Create();

        _hasher
            .Setup(x => x.Hash(It.IsAny<string>(), It.IsAny<int>()))
            .Returns("hashed");

        _tokenService
            .Setup(x => x.GenerateToken(It.IsAny<UserEntity>()))
            .Returns("token");

        var handler = CreateHandler(db);

        var query = new RegisterUserCommand(
            Email: "test@test.com",
            Password: "123456",
            FirstName: "Test",
            LastName: "User");

        await handler.Handle(query, TestContext.Current.CancellationToken);

        _tokenService.Verify(x =>
            x.GenerateToken(It.Is<UserEntity>(u =>
                u.Email == "test@test.com" &&
                u.PasswordHash == "hashed")),
            Times.Once);
    }
}