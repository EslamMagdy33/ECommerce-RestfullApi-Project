using FluentValidation;

namespace ECommerce.UseCases.Orders.Commands.CancelOrder;

public sealed class CancelOrderCommandValidator :AbstractValidator<CancelOrderCommand>
{
    public CancelOrderCommandValidator()
    {
        RuleFor(o=>o.OrderId)
            .NotEmpty()
            .WithErrorCode("Order.InValid")
            .WithMessage("Order Is Required.");
    }
}
