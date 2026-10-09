using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Messaging.Implementations;

internal sealed class Sender(HandlerExecutorRegistry registry) : ISender
{
    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var executor=registry.GetExecutor(request.GetType());
        var result= await executor.ExecuteAsync(request, ct).ConfigureAwait(false);
        if (result is null)
        {
            throw new InvalidOperationException(
                $"Handler for '{request.GetType().Name}' returned null, but {typeof(TResponse).Name} was expected.");
        }
        if (result is not TResponse typedResult)
            throw new InvalidOperationException(
                $"Handler for '{request.GetType().Name}' returned {result.GetType().Name}, but {typeof(TResponse).Name} was expected.");
        return typedResult;

    }
}
