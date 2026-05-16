using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zent.Data.Entities;

[Table("boards")]
public sealed class BoardEntity
{
    [Column("id")]
    public Guid Id { get; set; }
    
    [Column("project_id")]
    public Guid ProjectId { get; set; }
    
    [Column("name")]
    [MaxLength(32)]
    public required string Name { get; set; }
    
    [Column("description")]
    [MaxLength(128)]
    public string? Description { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public ProjectEntity Project { get; set; } = null!;
    public List<ColumnEntity> Columns { get; set; } = [];
}