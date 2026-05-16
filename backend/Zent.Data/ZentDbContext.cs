using Microsoft.EntityFrameworkCore;
using Zent.Data.Entities;

namespace Zent.Data;

public sealed class ZentDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<TeamEntity> Teams => Set<TeamEntity>();
    public DbSet<ProjectEntity> Projects => Set<ProjectEntity>();
    public DbSet<BoardEntity> Boards => Set<BoardEntity>();
    public DbSet<ColumnEntity> Columns => Set<ColumnEntity>();
    public DbSet<TaskEntity> Tasks => Set<TaskEntity>();
    
    // many-to-many
    public DbSet<TeamMemberEntity> TeamMembers => Set<TeamMemberEntity>();
    public DbSet<ProjectMemberEntity> ProjectMembers => Set<ProjectMemberEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ZentDbContext).Assembly);
    }
}