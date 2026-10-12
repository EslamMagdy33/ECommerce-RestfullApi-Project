using ECommerce.Domain.Common;
using ECommerce.Domain.Errors;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Common.Models;
using ECommerce.UseCases.Common.Settings;
using ECommerce.UseCases.Identity.Commands.Register;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace ECommerce.UseCases.Tests.Identity;

public class RegisterCommandHandlerTests
{
    private const string Email = "victim@example.com";

    private readonly IIdentityService identityService = Substitute.For<IIdentityService>();
    private readonly IEmailVerificationCodeStore codeStore = Substitute.For<IEmailVerificationCodeStore>();
    private readonly IEmailSender emailSender = Substitute.For<IEmailSender>();
    private readonly RegisterCommandHandler handler;

    public RegisterCommandHandlerTests()
    {
        var settings = Options.Create(new EmailVerificationSettings
        {
            CodeLength = 6,
            ExpirationMinutes = 15
        });

        emailSender.SendAsync(default!, default!, default!, default)
            .ReturnsForAnyArgs(Result.Success());

        handler = new RegisterCommandHandler(identityService, codeStore, emailSender, settings);
    }

    private static Result<AuthUserSnapshot> ExistingUser(string? displayName = null) =>
        Result<AuthUserSnapshot>.Success(new AuthUserSnapshot(Guid.NewGuid(), Email, displayName));

    [Fact]
    public async Task Unconfirmed_email_registration_replaces_the_stored_credentials()
    {
        identityService.GetUserByEmailAsync(Email, Arg.Any<CancellationToken>())
            .Returns(ExistingUser());
        identityService.IsEmailConfirmedAsync(Email, Arg.Any<CancellationToken>())
            .Returns(false);
        identityService.ReplaceUnconfirmedUserAsync(Email, "NewPass123", "New Name", Arg.Any<CancellationToken>())
            .Returns(ExistingUser("New Name"));

        var result = await handler.Handle(
            new RegisterCommand(Email: Email, Password: "NewPass123", DisplayName: "New Name"));

        Assert.True(result.IsSuccess);
        await identityService.Received(1)
            .ReplaceUnconfirmedUserAsync(Email, "NewPass123", "New Name", Arg.Any<CancellationToken>());
        await identityService.DidNotReceiveWithAnyArgs()
            .CreateUserAsync(default!, default!, default, default);
    }

    [Fact]
    public async Task Confirmed_email_registration_fails_and_changes_nothing()
    {
        identityService.GetUserByEmailAsync(Email, Arg.Any<CancellationToken>())
            .Returns(ExistingUser());
        identityService.IsEmailConfirmedAsync(Email, Arg.Any<CancellationToken>())
            .Returns(true);

        var result = await handler.Handle(
            new RegisterCommand(Email: Email, Password: "NewPass123", DisplayName: "New Name"));

        Assert.True(result.IsFailure);
        Assert.Equal(IdentityErrors.EmailAlreadyExists.Code, result.Error!.Code);
        await identityService.DidNotReceiveWithAnyArgs()
            .ReplaceUnconfirmedUserAsync(default!, default!, default, default);
        await emailSender.DidNotReceiveWithAnyArgs()
            .SendAsync(default!, default!, default!, default);
    }

    [Fact]
    public async Task New_email_registration_creates_a_user()
    {
        identityService.GetUserByEmailAsync(Email, Arg.Any<CancellationToken>())
            .Returns(Result<AuthUserSnapshot>.Failure(IdentityErrors.UserNotFound));
        identityService.CreateUserAsync(Email, "NewPass123", "New Name", Arg.Any<CancellationToken>())
            .Returns(ExistingUser("New Name"));

        var result = await handler.Handle(
            new RegisterCommand(Email: Email, Password: "NewPass123", DisplayName: "New Name"));

        Assert.True(result.IsSuccess);
        await identityService.Received(1)
            .CreateUserAsync(Email, "NewPass123", "New Name", Arg.Any<CancellationToken>());
        await identityService.DidNotReceiveWithAnyArgs()
            .ReplaceUnconfirmedUserAsync(default!, default!, default, default);
    }
}