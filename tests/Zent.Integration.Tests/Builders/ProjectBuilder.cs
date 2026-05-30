using Zent.Data.Entities;

namespace Zent.Integration.Tests.Builders;

public sealed class ProjectBuilder
{
    private Guid _teamId;
    private string _name = $"Project {Guid.NewGuid():N}"[..15];
    private string? _description = "Project description";
    private string? _client = "Client";
    private DateTime _createdAt = DateTime.UtcNow;

    public ProjectBuilder ForTeam(TeamEntity team)
    {
        _teamId = team.Id;
        return this;
    }

    public ProjectBuilder ForTeamId(Guid teamId)
    {
        _teamId = teamId;
        return this;
    }

    public ProjectBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public ProjectEntity Build()
        => new()
        {
            TeamId = _teamId,
            Name = _name,
            Description = _description,
            Client = _client,
            CreatedAt = _createdAt
        };
}
