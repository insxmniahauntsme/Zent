using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zent.Data.Entities;

namespace Zent.Data.Configurations;

internal sealed class ColumnEntityConfiguration : IEntityTypeConfiguration<ColumnEntity>
{
    public void Configure(EntityTypeBuilder<ColumnEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasMany(x => x.Tasks)
            .WithOne(x => x.Column)
            .HasForeignKey(x => x.ColumnId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.BoardId);

        builder.HasIndex(x => new { x.Title, x.BoardId }).IsUnique();
    }
}