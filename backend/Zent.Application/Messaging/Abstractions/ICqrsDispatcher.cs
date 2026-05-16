namespace Zent.Application.Messaging.Abstractions;

public interface ICqrsDispatcher
{
    Task Send(ICommand command, CancellationToken ct = default);

    Task<TResult> Send<TResult>(ICommand<TResult> command, CancellationToken ct = default);

    Task<TResult> Send<TResult>(IQuery<TResult> query, CancellationToken ct = default);
}