namespace ECommerce.UseCases.Messaging.Abstractions;

public interface IQuery<out TResponse> : IRequest<TResponse>;
