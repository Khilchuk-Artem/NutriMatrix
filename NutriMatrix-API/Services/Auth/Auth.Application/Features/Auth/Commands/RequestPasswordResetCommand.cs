using Auth.Application.Services.EmailService;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Web;

namespace Auth.Application.Features.Auth.Commands
{
    public class RequestPasswordResetCommand : IRequest<bool>
    {
        public string Email { get; set; }
    }

    public class RequestPasswordResetCommandHandler : IRequestHandler<RequestPasswordResetCommand, bool>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;

        public RequestPasswordResetCommandHandler(UserManager<IdentityUser> userManager, IConfiguration config, IEmailService emailService)
        {
            _userManager = userManager;
            _config = config;
            _emailService = emailService;
        }

        public async Task<bool> Handle(RequestPasswordResetCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
            {
                return false;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"{_config["ClientUrl"]}/auth/reset-password/confirm?token={HttpUtility.UrlEncode(token)}&email={HttpUtility.UrlEncode(request.Email)}";
            await _emailService.SendEmailAsync(request.Email, "Password reset", $"Reset your password by clicking <a href='{resetLink}'>here</a>");
            return true;
        }
    }
}