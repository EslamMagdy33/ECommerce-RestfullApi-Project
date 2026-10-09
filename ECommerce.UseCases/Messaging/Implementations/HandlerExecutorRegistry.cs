namespace ECommerce.UseCases.Messaging.Implementations;

public sealed class HandlerExecutorRegistry
{
    private readonly IReadOnlyDictionary<Type,IRequestHandlerExecutor> executors;

    public HandlerExecutorRegistry(IEnumerable<IRequestHandlerExecutor> executors)
    {
        this.executors = executors.ToDictionary(exe=>exe.RequestType);
    }

    public IRequestHandlerExecutor GetExecutor(Type requestType)
    {
        ArgumentNullException.ThrowIfNull(requestType);
        if (executors.TryGetValue(requestType, out var executor))
            return executor;
        throw new InvalidOperationException($"No handler registered for request type '{requestType.FullName}");
    }


}
