﻿using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using UserService.Application.Users.Commands.Create.UserService.Infrastructure.Email;
using UserService.Domain.Users;

namespace UserService.Application.Users.Commands.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ForgotPasswordResponse>
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<ForgotPasswordCommandHandler> _logger;

        public ForgotPasswordCommandHandler(UserManager<User> userManager, IEmailService emailService, ILogger<ForgotPasswordCommandHandler> logger)
        {
            _userManager = userManager;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<ForgotPasswordResponse> Handle(ForgotPasswordCommand command, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(command.Email);
            if (user == null)
            {
                _logger.LogWarning("Password reset requested for non-existent email: {Email}", command.Email);
                return new ForgotPasswordResponse(false, "No user found with that email address.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            _logger.LogInformation("Password reset token generated for user {UserId}: {Token}", user.Id, token);

            var resetLink = $"https://yourdomain.com/api/Account/reset-password?userId={user.Id}&token={Uri.EscapeDataString(token)}";
            var subject = "Reset your password";
            var body = $"Hello, {user.Name},<br><br>Please reset your password by clicking <a href='{resetLink}'>here</a>.";

            var emailSent = await _emailService.SendEmailAsync(user.Email, subject, body);
            if (!emailSent)
            {
                _logger.LogWarning("Failed to send password reset email to {Email}", user.Email);
                return new ForgotPasswordResponse(false, "Error sending password reset email.");
            }

            _logger.LogInformation("Password reset email sent successfully to {Email}", user.Email);
            return new ForgotPasswordResponse(true, "Password reset email sent successfully.");
        }
    }
}
