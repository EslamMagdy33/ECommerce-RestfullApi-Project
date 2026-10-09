using ECommerce.Domain.Common;
using ECommerce.UseCases.Identity.Dtos;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Identity.Commands.Login;

public sealed record LoginCommand(string Email, string Password) : ICommand<Result<AuthResponse>>
{
}
