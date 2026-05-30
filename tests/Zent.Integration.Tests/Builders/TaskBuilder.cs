using Zent.Common.Enums;
using Zent.Data.Entities;

namespace Zent.Integration.Tests.Builders;

public sealed class TaskBuilder
{
    private Guid _columnId;
    private Guid _creatorId;
    private Guid? _assigneeId;
    private string _title = $"Task {Guid.NewGuid():N}"[..12];
    private string? _description = "Task description";
    private TaskPriority _priority = TaskPriority.Medium;
    private int _order = 1;
    private DateTime _createdAt = DateTime.UtcNow;
    private DateOnly? _untilDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

    public TaskBuilder ForColumn(ColumnEntity column)
    {
        _columnId = column.Id;
        return this;
    }

    public TaskBuilder CreatedBy(UserEntity user)
    {
        _creatorId = user.Id;
        return this;
    }

    public TaskBuilder AssignedTo(UserEntity user)
    {
        _assigneeId = user.Id;
        return this;
    }

    public TaskBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public TaskEntity Build()
        => new()
        {
            ColumnId = _columnId,
            CreatorId = _creatorId,
            AssigneeId = _assigneeId,
            Title = _title,
            Description = _description,
            Priority = _priority,
            Order = _order,
            CreatedAt = _createdAt,
            UntilDate = _untilDate
        };
}
