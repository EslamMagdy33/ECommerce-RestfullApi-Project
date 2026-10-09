using ECommerce.Domain.Common;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Identity.Commands.ConfirmEmail;

public sealed record ConfirmEmailCommand(
    string Email,
    string Code) : ICommand<Result>;
