using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zent.Data.Entities;

namespace Zent.Data.Configurations;

internal sealed class ProjectMemberEntityConfiguration : IEntityTypeConfiguration<ProjectMemberEntity>
{
    public void Configure(EntityTypeBuilder<ProjectMemberEntity> builder)
    {
        builder.HasKey(x => new { x.UserId, x.ProjectId });
        
        builder.HasOne(x => x.User)
            .WithMany(x => x.ProjectMemberships)
            .HasForeignKey(x => x.UserId);
        
        builder.HasOne(x => x.Project)
            .WithMany(x => x.Members)
            .HasForeignKey(x => x.ProjectId);
    }
}