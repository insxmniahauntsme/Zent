using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zent.Data.Entities;

namespace Zent.Data.Configurations;

internal sealed class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.HasMany(x => x.TeamMemberships)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId);
        
        builder.HasMany(x => x.ProjectMemberships)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId);
        
        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasIndex(x => x.FirstName);
        builder.HasIndex(x => x.LastName);
    }
}