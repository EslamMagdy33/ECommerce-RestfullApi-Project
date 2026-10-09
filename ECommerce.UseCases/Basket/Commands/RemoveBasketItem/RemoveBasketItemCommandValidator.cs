using FluentValidation;

namespace ECommerce.UseCases.Basket.Commands.RemoveBasketItem;

public sealed class RemoveBasketItemCommandValidator : AbstractValidator<RemoveBasketItemCommand>
{
    public RemoveBasketItemCommandValidator()
    {
        RuleFor(x => x.BuyerId)
            .NotEmpty()
            .WithErrorCode("Basket.InvalidBuyerId")
            .WithMessage("A valid buyer id is required.");

        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithErrorCode("Basket.InvalidProductId")
            .WithMessage("Product id is required.");
    }
}
