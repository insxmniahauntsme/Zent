using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zent.Data.Entities;

namespace Zent.Data.Configurations;

internal sealed class TaskEntityConfiguration : IEntityTypeConfiguration<TaskEntity>
{
    public void Configure(EntityTypeBuilder<TaskEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Creator)
            .WithMany()
            .HasForeignKey(x => x.CreatorId);
        
        builder.HasOne(x => x.Assignee)
            .WithMany()
            .HasForeignKey(x => x.AssigneeId);
        
        builder.HasOne(x => x.Column)
            .WithMany(x => x.Tasks)
            .HasForeignKey(x => x.ColumnId);

        builder.HasIndex(x => x.ColumnId);
        
        builder.HasIndex(x => x.AssigneeId);
        
        builder.HasIndex(x => x.CreatorId);

        builder.HasIndex(x => x.CreatedAt);

        builder.HasIndex(x => x.UntilDate);
    }
}