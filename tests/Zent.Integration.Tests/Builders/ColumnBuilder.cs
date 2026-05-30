using Zent.Data.Entities;

namespace Zent.Integration.Tests.Builders;

public sealed class ColumnBuilder
{
    private Guid _boardId;
    private string _title = $"Column {Guid.NewGuid():N}"[..14];
    private int _order = 1;
    private bool _isFinal;

    public ColumnBuilder ForBoard(BoardEntity board)
    {
        _boardId = board.Id;
        return this;
    }

    public ColumnBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public ColumnBuilder WithOrder(int order)
    {
        _order = order;
        return this;
    }

    public ColumnBuilder AsFinal()
    {
        _isFinal = true;
        return this;
    }

    public ColumnEntity Build()
        => new()
        {
            BoardId = _boardId,
            Title = _title,
            Order = _order,
            IsFinal = _isFinal
        };
}
