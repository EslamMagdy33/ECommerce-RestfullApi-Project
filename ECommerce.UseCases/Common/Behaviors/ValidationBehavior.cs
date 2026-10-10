using ECommerce.Domain.Common;
using ECommerce.UseCases.Messaging.Abstractions;
using FluentValidation;
using System.Reflection;

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

        var responseType = typeof(TResponse);

        if (responseType == typeof(Result))
            return (TResponse)(object)Result.Failure(error);

        if (responseType.IsGenericType &&
            responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var staticMethods = responseType
                .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            var failure = staticMethods.FirstOrDefault(m =>
                m.Name == "Failure" && m.GetParameters().Length == 1);

            if (failure is not null)
                return (TResponse)failure.Invoke(null, new object[] { error })!;

            throw new InvalidOperationException(
                $"No Failure method on '{responseType}'. Static methods found: " +
                string.Join(", ", staticMethods.Select(m => m.Name)));
        }

        throw new InvalidOperationException(
            $"ValidationBehavior cannot create a failure result for '{responseType}'.");

    }
}
