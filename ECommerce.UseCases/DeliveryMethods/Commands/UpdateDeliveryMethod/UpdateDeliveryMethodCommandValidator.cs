using ECommerce.Domain.Entities;
using FluentValidation;

namespace ECommerce.UseCases.DeliveryMethods.Commands.UpdateDeliveryMethod;

public sealed class UpdateDeliveryMethodCommandValidator : AbstractValidator<UpdateDeliveryMethodCommand>
{
    public UpdateDeliveryMethodCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithErrorCode("DeliveryMethod.InvalidId")
            .WithMessage("Delivery method id is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithErrorCode("DeliveryMethod.InvalidName")
            .WithMessage("Delivery method name is required.")
            .MaximumLength(DeliveryMethod.MaxNameLength)
            .WithErrorCode("DeliveryMethod.NameTooLong")
            .WithMessage($"Delivery method name cannot exceed {DeliveryMethod.MaxNameLength} characters.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .WithErrorCode("DeliveryMethod.InvalidPrice")
            .WithMessage("Delivery price cannot be negative.");

        RuleFor(x => x.EstimatedDeliveryTime)
            .NotEmpty()
            .WithErrorCode("DeliveryMethod.InvalidDeliveryTime")
            .WithMessage("Estimated delivery time is required.")
            .MaximumLength(DeliveryMethod.MaxDeliveryTimeLength)
            .WithErrorCode("DeliveryMethod.InvalidDeliveryTime")
            .WithMessage($"Estimated delivery time cannot exceed {DeliveryMethod.MaxDeliveryTimeLength} characters.");

        RuleFor(x => x.Description)
            .MaximumLength(DeliveryMethod.MaxDescriptionLength)
            .When(x => !string.IsNullOrWhiteSpace(x.Description))
            .WithErrorCode("DeliveryMethod.DescriptionTooLong")
            .WithMessage($"Delivery method description cannot exceed {DeliveryMethod.MaxDescriptionLength} characters.");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .WithErrorCode("DeliveryMethod.InvalidDisplayOrder")
            .WithMessage("Display order cannot be negative.");
    }
}
