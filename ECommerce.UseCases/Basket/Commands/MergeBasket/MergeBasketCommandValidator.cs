using FluentValidation;

namespace ECommerce.UseCases.Basket.Commands.MergeBasket;

public sealed class MergeBasketCommandValidator : AbstractValidator<MergeBasketCommand>
{
    public MergeBasketCommandValidator()
    {
        RuleFor(x => x.BuyerId)
            .NotEmpty()
            .WithErrorCode("Basket.InvalidBuyerId")
            .WithMessage("A valid buyer id is required.");

        RuleFor(x => x.AnonymousBuyerId)
            .NotEmpty()
            .WithErrorCode("Basket.AnonymousBuyerRequired")
            .WithMessage("Anonymous buyer id is required for merge.");

        RuleFor(x => x)
            .Must(x => x.BuyerId != x.AnonymousBuyerId)
            .WithErrorCode("Basket.CannotMergeSameBuyer")
            .WithMessage("Cannot merge a basket into itself.");
    }
}
