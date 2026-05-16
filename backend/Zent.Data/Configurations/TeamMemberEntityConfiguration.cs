using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zent.Data.Entities;

namespace Zent.Data.Configurations;

internal sealed class TeamMemberEntityConfiguration : IEntityTypeConfiguration<TeamMemberEntity>
{
    public void Configure(EntityTypeBuilder<TeamMemberEntity> builder)
    {
        builder.HasKey(x => new { x.UserId, x.TeamId });
        
        builder.HasOne(x => x.User)
            .WithMany(x => x.TeamMemberships)
            .HasForeignKey(x => x.UserId);
        
        builder.HasOne(x => x.Team)
            .WithMany(x => x.TeamMemberships)
            .HasForeignKey(x => x.TeamId);
        
        builder.Property(x => x.MemberRole)
            .HasConversion<string>();
        
        builder.Property(x => x.Specialization)
            .HasConversion<string>();
    }
}