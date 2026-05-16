using Zent.Common.Enums;
using Zent.Data.Entities;

namespace Zent.Data;

public static class TeamMemberQueryExtensions
{
    extension(IQueryable<TeamMemberEntity> query)
    {
        public IQueryable<TeamMemberEntity> WithProjectManageAccess()
        {
            return query.Where(x =>
                x.MemberRole == TeamRole.Owner ||
                x.MemberRole == TeamRole.Admin);
        }
        
        public IQueryable<TeamMemberEntity> WithBoardManageAccess()
        {
            return query.Where(x =>
                x.MemberRole == TeamRole.Owner ||
                x.MemberRole == TeamRole.Admin);
        }
    }
}