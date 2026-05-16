namespace Zent.Application.Messaging.Abstractions;

public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
{
    public Task<TResult> Handle(TQuery query, CancellationToken ct);
}