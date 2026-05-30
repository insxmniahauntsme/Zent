using Zent.Data.Entities;

namespace Zent.Integration.Tests.Builders;

public sealed class TeamBuilder
{
    private Guid _ownerId;
    private string _name = $"Team {Guid.NewGuid():N}"[..12];

    public TeamBuilder WithOwner(UserEntity owner)
    {
        _ownerId = owner.Id;
        return this;
    }

    public TeamBuilder WithOwnerId(Guid ownerId)
    {
        _ownerId = ownerId;
        return this;
    }

    public TeamBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public TeamEntity Build()
        => new()
        {
            OwnerId = _ownerId,
            Name = _name
        };
}
