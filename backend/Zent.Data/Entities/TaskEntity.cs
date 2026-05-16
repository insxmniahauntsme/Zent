using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zent.Common.Enums;

namespace Zent.Data.Entities;

[Table("tasks")]
public sealed class TaskEntity
{
    [Column("id")]
    public Guid Id { get; set; }
    
    [Column("column_id")]
    public Guid ColumnId { get; set; }
    
    [Column("creator_id")]
    public Guid CreatorId { get; set; }
    
    [Column("assignee_id")]
    public Guid? AssigneeId { get; set; }
    
    [Column("title")]
    [MaxLength(64)]
    public required string Title { get; set; }
    
    [Column("description")]
    [MaxLength(512)]
    public string? Description { get; set; }
    
    [Column("priority")]
    public TaskPriority Priority { get; set; }
    
    [Column("order")]
    public int Order { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Column("until_date")]
    public DateOnly? UntilDate { get; set; }
    
    // Navigation properties
    public UserEntity Creator { get; set; } = null!;
    public ColumnEntity Column { get; set; } = null!;
    public UserEntity? Assignee { get; set; }
}