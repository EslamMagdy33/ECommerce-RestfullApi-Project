using FluentValidation;

namespace ECommerce.UseCases.DeliveryMethods.Commands.DeleteDeliveryMethod;

public sealed class DeleteDeliveryMethodCommandValidator : AbstractValidator<DeleteDeliveryMethodCommand>
{
    public DeleteDeliveryMethodCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithErrorCode("DeliveryMethod.InvalidId")
            .WithMessage("Delivery method id is required.");
    }
}
