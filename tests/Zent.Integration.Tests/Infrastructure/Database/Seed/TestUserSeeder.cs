using Microsoft.EntityFrameworkCore;
using Zent.Data;
using Zent.Data.Entities;

namespace Zent.Integration.Tests.Infrastructure.Database.Seed;

public sealed class TestUserSeeder
{
    public const string Email = "test-user-runner@example.com";
    public const string Password = "123456";
    
    public async Task SeedAsync(ZentDbContext dbContext)
    {
        if (await dbContext.Users.AnyAsync(x => x.Email == "test-user-runner@example.com"))
            return;
        
        var userEntity = new UserEntity
        {
            Email = Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(Password, BCrypt.Net.BCrypt.GenerateSalt(13)),
            FirstName = "Test",
            LastName = "User"
        };
        
        dbContext.Users.Add(userEntity);
        await dbContext.SaveChangesAsync();
    }
}