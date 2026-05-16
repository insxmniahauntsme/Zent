using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zent.Data.Entities;

[Table("projects")]
public sealed class ProjectEntity
{
    [Column("id")]
    public Guid Id { get; set; }
    
    [Column("team_id")]
    public Guid TeamId { get; set; }
    
    [Column("name")]
    [MaxLength(32)]
    public required string Name { get; set; }
    
    [Column("description")]
    [MaxLength(128)]
    public string? Description { get; set; }
    
    [Column("client")]
    [MaxLength(32)]
    public string? Client { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public TeamEntity Team { get; set; } = null!;
    public List<ProjectMemberEntity> Members { get; set; } = [];
    public List<BoardEntity> Boards { get; set; } = [];
}