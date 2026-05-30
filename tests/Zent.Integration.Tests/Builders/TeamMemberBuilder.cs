using Zent.Common.Enums;
using Zent.Data.Entities;

namespace Zent.Integration.Tests.Builders;

public sealed class TeamMemberBuilder
{
    private Guid _userId;
    private Guid _teamId;
    private TeamRole _role = TeamRole.Member;

    public TeamMemberBuilder ForUser(UserEntity user)
    {
        _userId = user.Id;
        return this;
    }

    public TeamMemberBuilder ForUserId(Guid userId)
    {
        _userId = userId;
        return this;
    }

    public TeamMemberBuilder ForTeam(TeamEntity team)
    {
        _teamId = team.Id;
        return this;
    }

    public TeamMemberBuilder ForTeamId(Guid teamId)
    {
        _teamId = teamId;
        return this;
    }

    public TeamMemberBuilder WithRole(TeamRole role)
    {
        _role = role;
        return this;
    }

    public TeamMemberEntity Build()
        => new()
        {
            UserId = _userId,
            TeamId = _teamId,
            MemberRole = _role
        };
}
