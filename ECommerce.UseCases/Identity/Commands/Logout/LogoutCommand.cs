using ECommerce.Domain.Common;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Identity.Commands.Logout;

public sealed record LogoutCommand(string RefreshToken) : ICommand<Result>;
