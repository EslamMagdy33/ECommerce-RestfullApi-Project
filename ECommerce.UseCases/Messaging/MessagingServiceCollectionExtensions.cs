using ECommerce.UseCases.Common.Behaviors;
using ECommerce.UseCases.Messaging.Abstractions;
using ECommerce.UseCases.Messaging.Implementations;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ECommerce.UseCases.Messaging;

public static class MessagingServiceCollectionExtensions
{
    public static IServiceCollection AddMessaging(this IServiceCollection services,Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assembly);

        RegisterHandlers(services,assembly);

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped<ISender, Sender>();
        services.AddScoped<HandlerExecutorRegistry>();

        return services;
    }
    public static void RegisterHandlers(IServiceCollection services,Assembly assembly)
    {
        var HandlerInterface= typeof(IRequestHandle<,>);
        foreach (var handlerType in assembly.GetTypes())
        {
            if (handlerType is not { IsClass: true, IsAbstract: false })
                continue;

            foreach (var serviceInterface in handlerType.GetInterfaces())
            {
                if (!serviceInterface.IsGenericType || serviceInterface.GetGenericTypeDefinition() != HandlerInterface)
                {
                    continue;
                }
                var requestType = serviceInterface.GetGenericArguments()[0];
                var responseType = serviceInterface.GetGenericArguments()[1];

                services.AddScoped(serviceInterface, handlerType);

                var executorType = typeof(RequestHandlerExecutor<,>).MakeGenericType(requestType, responseType);
                services.AddScoped(typeof(IRequestHandlerExecutor),executorType);
            }
        }
    }
}
