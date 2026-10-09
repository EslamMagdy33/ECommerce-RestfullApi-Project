namespace ECommerce.UseCases.Messaging.Implementations;

public interface IRequestHandlerExecutor
{
    Type RequestType { get; }
    Task<object?> ExecuteAsync(object request,CancellationToken ct=default);
}
