using FluentValidation;

namespace ECommerce.UseCases.Basket.Commands.ClearBasket;

public sealed class ClearBasketCommandValidator : AbstractValidator<ClearBasketCommand>
{
    public ClearBasketCommandValidator()
    {
        RuleFor(x => x.BuyerId)
            .NotEmpty()
            .WithErrorCode("Basket.InvalidBuyerId")
            .WithMessage("A valid buyer id is required.");
    }
}
