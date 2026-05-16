using System.ComponentModel.DataAnnotations.Schema;
using Zent.Common.Enums;

namespace Zent.Data.Entities;

[Table("team_members")]
public sealed class TeamMemberEntity
{
    [Column("user_id")]
    public Guid UserId { get; set; }
    
    [Column("team_id")]
    public Guid TeamId { get; set; }
    
    [Column("member_role")]
    public TeamRole MemberRole { get; set; }
    
    [Column("specialization")]
    public Specialization? Specialization { get; set; }
    
    // Navigation properties
    public UserEntity User { get; set; } = null!;
    public TeamEntity Team { get; set; } = null!;
}