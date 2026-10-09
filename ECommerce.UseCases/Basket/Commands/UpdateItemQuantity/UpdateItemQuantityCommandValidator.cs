using ECommerce.Domain.Entities;
using FluentValidation;

namespace ECommerce.UseCases.Basket.Commands.UpdateItemQuantity;

public sealed class UpdateItemQuantityCommandValidator : AbstractValidator<UpdateItemQuantityCommand>
{
    public UpdateItemQuantityCommandValidator()
    {
        RuleFor(x => x.BuyerId)
            .NotEmpty()
            .WithErrorCode("Basket.InvalidBuyerId")
            .WithMessage("A valid buyer id is required.");

        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithErrorCode("Basket.InvalidProductId")
            .WithMessage("Product id is required.");

        RuleFor(x => x.Quantity)
            .InclusiveBetween(BasketItem.MinQuantity, BasketItem.MaxQuantity)
            .WithErrorCode("Basket.InvalidQuantity")
            .WithMessage($"Quantity must be between {BasketItem.MinQuantity} and {BasketItem.MaxQuantity}.");
    }
}
