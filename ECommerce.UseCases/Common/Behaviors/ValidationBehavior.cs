using ECommerce.Domain.Common;
using ECommerce.UseCases.Messaging.Abstractions;
using FluentValidation;

namespace ECommerce.UseCases.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, Func<Task<TResponse>> next, CancellationToken ct = default)
    {
        if (!validators.Any())
            return await next();

        var context=new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, ct)));

        var failures = validationResults
            .SelectMany(r => r.Errors).ToList();

        if (failures.Count == 0)
            return await next();

        var firstFailure = failures[0];
        var error = Error.Validation(
            string.IsNullOrEmpty(firstFailure.ErrorCode) ? "Validation.Failed" : firstFailure.ErrorCode,
            firstFailure.ErrorMessage);

        dynamic dynamicResponse = error;
        return (TResponse)dynamicResponse;

    }
}
