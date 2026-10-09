using FluentValidation;

namespace ECommerce.UseCases.Identity.Commands.RefreshToken;

public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithErrorCode("Identity.InvalidRefreshToken")
            .WithMessage("Refresh token is required.");
    }
}
