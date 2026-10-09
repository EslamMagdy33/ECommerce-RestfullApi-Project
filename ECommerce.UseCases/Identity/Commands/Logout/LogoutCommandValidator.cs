using FluentValidation;

namespace ECommerce.UseCases.Identity.Commands.Logout;

public sealed class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithErrorCode("Identity.InvalidRefreshToken")
            .WithMessage("Refresh token is required.");
    }
}
