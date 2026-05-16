using Microsoft.Extensions.DependencyInjection;
using Zent.Application.Messaging.Abstractions;

namespace Zent.Application.Messaging;

public sealed class CqrsDispatcher(IServiceProvider services) : ICqrsDispatcher
{
    public async Task Send(ICommand command, CancellationToken ct = default)
    {
        var commandType = command.GetType();
        var handlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);

        try
        {
            var handler = services.GetRequiredService(handlerType);
            var method = handlerType.GetMethod("Handle");

            var task = (Task)method!.Invoke(handler, [command, ct])!;
            await task;
        }
        catch (InvalidOperationException)
        {
            throw new InvalidOperationException(
                $"No handler registered for command {commandType.Name}.");
        }
    }

    public async Task<TResult> Send<TResult>(ICommand<TResult> command, CancellationToken ct = default)
    {
        var commandType = command.GetType();
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(commandType, typeof(TResult));

        try
        {
            var handler = services.GetRequiredService(handlerType);
            var method = handlerType.GetMethod("Handle");

            var task = (Task<TResult>)method!.Invoke(handler, [command, ct])!;
            return await task;
        }
        catch (InvalidOperationException)
        {
            throw new InvalidOperationException(
                $"No handler registered for command {commandType.Name} with result {typeof(TResult).Name}.");
        }
    }

    public async Task<TResult> Send<TResult>(IQuery<TResult> query, CancellationToken ct = default)
    {
        var queryType = query.GetType();
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, typeof(TResult));

        try
        {
            var handler = services.GetRequiredService(handlerType);
            var method = handlerType.GetMethod("Handle");

            var task = (Task<TResult>)method!.Invoke(handler, [query, ct])!;
            return await task;
        }
        catch (InvalidOperationException)
        {
            throw new InvalidOperationException(
                $"No handler registered for query {queryType.Name}.");
        }
    }
}