using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zent.Data.Entities;

[Table("users")]
public sealed class UserEntity
{
    [Column("id")]
    public Guid Id { get; set; }
    
    [Column("first_name")]
    [MaxLength(32)]
    public required string FirstName { get; set; }
    
    [Column("last_name")]
    [MaxLength(32)]
    public required string LastName { get; set; }
    
    [Column("email")]
    [MaxLength(64)]
    public required string Email { get; set; }
    
    [Column("password_hash")]
    [MaxLength(128)]
    public required string PasswordHash { get; set; }
    
    // Navigation properties
    public List<TeamMemberEntity> TeamMemberships { get; set; } = [];
    public List<ProjectMemberEntity> ProjectMemberships { get; set; } = [];
}