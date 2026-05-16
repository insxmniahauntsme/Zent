using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zent.Data.Entities;

[Table("columns")]
public sealed class ColumnEntity
{
    [Column("id")]
    public Guid Id { get; set; }
    
    [Column("board_id")]
    public Guid BoardId { get; set; }
    
    [Column("title")]
    [MaxLength(32)]
    public required string Title { get; set; }
    
    [Column("order")]
    public int Order { get; set; }
    
    [Column("is_final")]
    public bool IsFinal { get; set; }

    // Navigation properties
    public List<TaskEntity> Tasks { get; set; } = [];
    public BoardEntity Board { get; set; } = null!;
}