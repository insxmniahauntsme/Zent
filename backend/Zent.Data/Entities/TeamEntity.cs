using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zent.Data.Entities;

[Table("teams")]
public sealed class TeamEntity
{
    [Column("id")]
    public Guid Id { get; set; }
    
    [Column("owner_id")]
    public Guid OwnerId { get; set; }
    
    [Column("name")]
    [MaxLength(32)]
    public required string Name { get; set; }
    
    // Navigation properties
    public List<TeamMemberEntity> TeamMemberships { get; set; } = [];
    public List<ProjectEntity> Projects { get; set; } = [];
}