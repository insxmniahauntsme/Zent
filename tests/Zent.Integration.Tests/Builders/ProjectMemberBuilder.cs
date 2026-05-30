using Zent.Data.Entities;

namespace Zent.Integration.Tests.Builders;

public sealed class ProjectMemberBuilder
{
    private Guid _userId;
    private Guid _projectId;

    public ProjectMemberBuilder ForUser(UserEntity user)
    {
        _userId = user.Id;
        return this;
    }

    public ProjectMemberBuilder ForUserId(Guid userId)
    {
        _userId = userId;
        return this;
    }

    public ProjectMemberBuilder ForProject(ProjectEntity project)
    {
        _projectId = project.Id;
        return this;
    }

    public ProjectMemberBuilder ForProjectId(Guid projectId)
    {
        _projectId = projectId;
        return this;
    }

    public ProjectMemberEntity Build()
        => new()
        {
            UserId = _userId,
            ProjectId = _projectId
        };
}
