using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Zent.Application.Features.Users.GetCurrentUser;
using Zent.Common.Exceptions;
using Zent.Data.Entities;
using Zent.Unit.Tests.Builders;

namespace Zent.Unit.Tests.Users;

public sealed class GetCurrentUserHandlerTests
{
    [Fact]
    public async Task Should_ReturnCurrentUser_WhenUserExists()
    {
        await using var db = TestDbContextFactory.Create();
        var user = await SeedUserAsync(db);
        var handler = new GetCurrentUserHandler(db);

        var result = await handler.Handle(
            new GetCurrentUserQuery(user.Id),
            TestContext.Current.CancellationToken);

        result.Id.Should().Be(user.Id);
        result.Email.Should().Be(user.Email);
        result.FirstName.Should().Be(user.FirstName);
        result.LastName.Should().Be(user.LastName);
    }

    [Fact]
    public async Task Should_Throw_WhenUserDoesNotExist()
    {
        await using var db = TestDbContextFactory.Create();
        var handler = new GetCurrentUserHandler(db);

        var act = () => handler.Handle(
            new GetCurrentUserQuery(Guid.NewGuid()),
            TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<UserNotFoundException>();
    }

    private static async Task<UserEntity> SeedUserAsync(Zent.Data.ZentDbContext db)
    {
        var user = new UserBuilder()
            .WithEmail($"user-{Guid.NewGuid()}@example.com")
            .WithName("Test", "User")
            .Build();

        db.Users.Add(user);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        return await db.Users
            .AsNoTracking()
            .SingleAsync(x => x.Email == user.Email, TestContext.Current.CancellationToken);
    }
}
