using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using NetArchTest.Rules;
using Shouldly;
using Xunit;

namespace ECommerce.ArchitectureTests;

public class LayerDependencyTests
{
    private static readonly Assembly Domain = typeof(ECommerce.Domain.Common.Result).Assembly;
    private static readonly Assembly UseCases = typeof(ECommerce.UseCases.Messaging.Abstractions.ISender).Assembly;

    [Fact]
    public void Domain_does_not_depend_on_outer_layers()
    {
        var result = Types.InAssembly(Domain)
            .ShouldNot()
            .HaveDependencyOnAny("ECommerce.UseCases", "ECommerce.Infrastructure", "ECommerce.API")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(string.Join(", ", result.FailingTypeNames ?? []));
    }

    [Fact]
    public void UseCases_does_not_depend_on_Infrastructure_or_API()
    {
        var result = Types.InAssembly(UseCases)
            .ShouldNot()
            .HaveDependencyOnAny("ECommerce.Infrastructure", "ECommerce.API")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(string.Join(", ", result.FailingTypeNames ?? []));
    }

    [Fact]
    public void UseCases_does_not_depend_on_AspNetCore_Http()
    {
        var result = Types.InAssembly(UseCases)
            .ShouldNot()
            .HaveDependencyOn("Microsoft.AspNetCore.Http")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(string.Join(", ", result.FailingTypeNames ?? []));
    }

    [Fact]
    public void All_validators_are_registered()
    {
        var services = new ServiceCollection();
        ECommerce.UseCases.DependencyInjection.AddApplication(services);

        var validators = UseCases.GetTypes()
            .Where(t => !t.IsAbstract && t.BaseType is { IsGenericType: true } b &&
                        b.GetGenericTypeDefinition() == typeof(AbstractValidator<>));

        foreach (var validator in validators)
        {
            var validated = validator.BaseType!.GetGenericArguments()[0];
            var serviceType = typeof(IValidator<>).MakeGenericType(validated);

            services.Any(d => d.ServiceType == serviceType && d.ImplementationType == validator)
                .ShouldBeTrue($"{validator.Name} is not registered");
        }
    }
}