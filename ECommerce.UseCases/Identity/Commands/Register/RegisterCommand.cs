using ECommerce.Domain.Common;
using ECommerce.UseCases.Identity.Dtos;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Identity.Commands.Register;

public sealed record RegisterCommand(
    string Email,
    string Password,
    string? DisplayName):ICommand<Result<EmailSentResponse>>;
