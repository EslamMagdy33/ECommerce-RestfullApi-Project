using FluentValidation;

namespace ECommerce.UseCases.Orders.Commands.CreateOrder;

public sealed class CreateOrderCommandValidator:AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(o=>o.ShippingAddressId)
            .NotEmpty()
            .WithErrorCode("Order.ShippingAddress.Required")
            .WithMessage("Shipping Adress is Required");
        RuleFor(o => o.DeliverMethodId)
            .NotEmpty()
            .WithErrorCode("Order.DeliverMethodId.Required")
            .WithMessage("Deliver MethodId is Required");
    }
}
