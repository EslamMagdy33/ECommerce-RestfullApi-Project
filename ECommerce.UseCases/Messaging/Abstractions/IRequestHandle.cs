namespace ECommerce.UseCases.Messaging.Abstractions;

public interface IRequestHandle<in TRequest,TResponse> where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken ct = default);
}
