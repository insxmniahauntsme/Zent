using Zent.Data.Entities;

namespace Zent.Unit.Tests.Builders;

public sealed class BoardBuilder
{
    private Guid _projectId;
    private string _name = $"Board {Guid.NewGuid():N}"[..13];
    private string? _description = "Board description";
    private DateTime _createdAt = DateTime.UtcNow;
    private DateTime _updatedAt = DateTime.UtcNow;

    public BoardBuilder ForProject(ProjectEntity project)
    {
        _projectId = project.Id;
        return this;
    }

    public BoardBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public BoardEntity Build()
        => new()
        {
            ProjectId = _projectId,
            Name = _name,
            Description = _description,
            CreatedAt = _createdAt,
            UpdatedAt = _updatedAt
        };
}
