using System.ComponentModel.DataAnnotations.Schema;

namespace Zent.Data.Entities;

[Table("project_members")]
public sealed class ProjectMemberEntity
{
    [Column("user_id")]
    public Guid UserId { get; set; }
    
    [Column("project_id")]
    public Guid ProjectId { get; set; }
    
    // Navigation properties
    public UserEntity User { get; set; } = null!;
    public ProjectEntity Project { get; set; } = null!;
}