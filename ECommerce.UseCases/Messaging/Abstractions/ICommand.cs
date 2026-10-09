namespace ECommerce.UseCases.Messaging.Abstractions;

public interface ICommand<out TResponse> : IRequest<TResponse>;
