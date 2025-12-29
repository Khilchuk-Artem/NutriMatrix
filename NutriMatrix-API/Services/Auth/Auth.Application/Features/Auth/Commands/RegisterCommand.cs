using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Web;
using MediatR;
using Microsoft.Extensions.Configuration;
using Auth.Application.DTO;
using Auth.Application.Services.EmailService;
using Auth.Domain.Contracts;
using Auth.Domain.Entities;

namespace Auth.Application.Features.Auth.Commands
{
    public class RegisterCommand : IRequest<IdentityResult>
    {
        public RegisterUserDTO RegisterUserDTO { get; set; }
    }

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, IdentityResult>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;
        private readonly IRepository<NutrientTracking> _nutrientTrackingRepository;

        public RegisterCommandHandler(
            UserManager<IdentityUser> userManager,
            IConfiguration config,
            IEmailService emailService,
            IRepository<NutrientTracking> nutrientTrackingRepository)
        {
            _userManager = userManager;
            _config = config;
            _emailService = emailService;
            _nutrientTrackingRepository = nutrientTrackingRepository;
        }

        public async Task<IdentityResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var dto = request.RegisterUserDTO;
            var identityUser = new IdentityUser
            {
                UserName = dto.Name,
                Email = dto.Email
            };

            var identityResult = await _userManager.CreateAsync(identityUser, dto.Password);
            if (!identityResult.Succeeded)
            {
                return identityResult;
            }

            if (dto.Roles != null && dto.Roles.Any())
            {
                identityResult = await _userManager.AddToRolesAsync(identityUser, dto.Roles);
                if (!identityResult.Succeeded)
                {
                    return identityResult;
                }
            }

            var nutrientIds = new List<long> { 301, 205, 601, 208, 606, 204, 605, 303, 291, 306, 307, 203, 269, 539, 324, 299, 1001, 1006, 1002, 290, 261, 260, 1003, 1004, 1005, 513, 221, 511, 207, 514, 454, 262, 639, 322, 321, 326, 421, 334, 312, 507, 268, 325, 610, 611, 696, 612, 625, 652, 697, 613, 626, 673, 662, 653, 687, 614, 617, 674, 663, 859, 618, 670, 675, 669, 619, 851, 685, 627, 615, 628, 672, 689, 852, 853, 620, 855, 629, 857, 624, 630, 858, 631, 621, 654, 671, 607, 608, 609, 645, 646, 693, 695, 313, 417, 431, 435, 432, 212, 287, 515, 211, 516, 512, 521, 503, 213, 504, 338, 337, 505, 214, 506, 304, 428, 315, 406, 573, 578, 257, 664, 676, 856, 665, 666, 305, 410, 508, 636, 517, 319, 405, 317, 518, 641, 209, 638, 210, 263, 404, 502, 323, 341, 343, 342, 501, 509, 510, 318, 320, 418, 415, 401, 328, 430, 429, 255, 309, 344, 345, 346, 347 };
            var nutrients = nutrientIds.Select(id => new NutrientTracking
            {
                NutrientId = id,
                UserId = identityUser.Id,
                TargetAmount = 0,
                IsActive = false
            }).ToList();

            foreach (var nutrient in nutrients)
            {
                await _nutrientTrackingRepository.Add(nutrient);
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
            var confirmationLink = $"{_config["ClientUrl"]}/auth/confirm-email?token={HttpUtility.UrlEncode(token)}&email={HttpUtility.UrlEncode(dto.Email)}";
            await _emailService.SendEmailAsync(dto.Email, "Email confirmation", $"Please confirm your email by clicking <a href='{confirmationLink}'>here</a>");

            return IdentityResult.Success;
        }
    }
}