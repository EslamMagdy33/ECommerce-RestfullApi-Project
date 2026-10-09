using ECommerce.Domain.Common;
using ECommerce.UseCases.Identity.Dtos;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Identity.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken):ICommand<Result<AuthResponse>>;
