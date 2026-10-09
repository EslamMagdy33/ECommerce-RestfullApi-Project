using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Messaging.Implementations;

internal sealed class RequestHandlerExecutor<TRequest, TResponse>
    (IRequestHandle<TRequest, TResponse> handler, IEnumerable<IPipelineBehavior<TRequest, TResponse>> behaviors) 
    : IRequestHandlerExecutor where TRequest : IRequest<TResponse>
{
    public Type RequestType =>typeof(TRequest);

    public async Task<object?> ExecuteAsync(object request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if(request is not TRequest typedRequest)
            throw new ArgumentException("Invalid request type", nameof(request));

        Func<Task<TResponse>> pipeline = () => handler.Handle(typedRequest, ct);
        foreach (var behavior in behaviors.Reverse())
        {
            var next = pipeline;
            pipeline=() => behavior.Handle(typedRequest, next, ct);
        }

        return await pipeline().ConfigureAwait(false);
    }
}
