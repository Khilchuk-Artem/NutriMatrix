using Auth.API.Data;
using Auth.API.Models;
using Auth.API.Models.DTO;
using Auth.API.Services.EmailService;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace Auth.API.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;
        private readonly AuthDbContext _dbContext;
        public AuthService(UserManager<IdentityUser> userManager, IConfiguration config, IEmailService emailService, AuthDbContext dbContext)
        {
            _userManager = userManager;
            _config = config;
            _emailService = emailService;
            _dbContext = dbContext;
        }

        public async Task<string> CreateJWTToken(IdentityUser user, List<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("userId", user.Id),
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<LoginResponseDTO> Login(LoginUserDTO loginUserDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginUserDTO.Email);

            if (user != null)
            {
                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginUserDTO.Password);
                if (passwordCheck)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var n = await _dbContext.NutrientTrackings.Where(r => r.UserId == user.Id).ToArrayAsync();



                    if (roles != null)
                    {
                        var response = new LoginResponseDTO()
                        {
                            Name = user.UserName,
                            Email = user.Email,
                            UserId = user.Id,
                            Roles = roles.ToArray(),
                            Token = await CreateJWTToken(user, roles.ToList()),
                            NutrientTrackings = n
                        };

                        return response;
                    }
                }
            }
            return null;
        }

        public async Task<IdentityResult> Register(RegisterUserDTO registerUserDTO)
        {
            var identityUser = new IdentityUser()
            {
                UserName = registerUserDTO.Name,
                Email = registerUserDTO.Email,
            };

            var identityResult = await _userManager.CreateAsync(identityUser, registerUserDTO.Password);
            if (identityResult.Succeeded)
            {
                if (registerUserDTO.Roles != null && registerUserDTO.Roles.Any())
                {
                    identityResult = await _userManager.AddToRolesAsync(identityUser, registerUserDTO.Roles);

                    if (identityResult.Succeeded)
                    {
                        var claim = new Claim("bio", "Empty");
                        identityResult = await _userManager.AddClaimAsync(identityUser, claim);
                        if (identityResult.Succeeded)
                        {
                            var nuteintIds = new List<long> { 301, 205, 601, 208, 606, 204, 605, 303, 291, 306, 307, 203, 269, 539, 324, 299, 1001, 1006, 1002, 290, 261, 260, 1003, 1004, 1005, 513, 221, 511, 207, 514, 454, 262, 639, 322, 321, 326, 421, 334, 312, 507, 268, 325, 610, 611, 696, 612, 625, 652, 697, 613, 626, 673, 662, 653, 687, 614, 617, 674, 663, 859, 618, 670, 675, 669, 619, 851, 685, 627, 615, 628, 672, 689, 852, 853, 620, 855, 629, 857, 624, 630, 858, 631, 621, 654, 671, 607, 608, 609, 645, 646, 693, 695, 313, 417, 431, 435, 432, 212, 287, 515, 211, 516, 512, 521, 503, 213, 504, 338, 337, 505, 214, 506, 304, 428, 315, 406, 573, 578, 257, 664, 676, 856, 665, 666, 305, 410, 508, 636, 517, 319, 405, 317, 518, 641, 209, 638, 210, 263, 404, 502, 323, 341, 343, 342, 501, 509, 510, 318, 320, 418, 415, 401, 328, 430, 429, 255, 309, 344, 345, 346, 347 };

                            var nutrients = nuteintIds.Select(id =>
                            {
                                return new NutrientTracking()
                                {
                                    NutrientId = id,
                                    UserId = identityUser.Id,
                                    TargetAmount = 0,
                                    IsActive = false
                                };
                            }).ToList();

                            await _dbContext.NutrientTrackings.AddRangeAsync(nutrients);
                            await _dbContext.SaveChangesAsync();

                            var token = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
                            var confirmationLink = $"{_config["ClientUrl"]}/auth/confirm-email?token={HttpUtility.UrlEncode(token)}&email={HttpUtility.UrlEncode(registerUserDTO.Email)}";

                            await _emailService.SendEmailAsync(registerUserDTO.Email, "Email confirmation", $"Please confirm your email by clicking <a href='{confirmationLink}'>here</a>");

                            return identityResult;
                        }
                    }
                }
            }
            return null;
        }

        public async Task<bool> ConfirmEmail(RequestConfirmEmailDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null) return false;

            var result = await _userManager.ConfirmEmailAsync(user, dto.Token);

            return result.Succeeded;
        }

        public async Task<bool> RequestPasswordReset(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user))) return false;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink = $"{_config["ClientUrl"]}/auth/reset-password/confirm?token={HttpUtility.UrlEncode(token)}&email={HttpUtility.UrlEncode(email)}";

            await _emailService.SendEmailAsync(email, "Password reset",$"Reset your password by clicking <a href='{resetLink}'>here</a>");

            return true;
        }

        public async Task<bool> ResetPassword(ResetPasswordDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return false;

            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);

            return result.Succeeded;
        }

        public async Task<string> GoogleLogin(string idToken)
        {
            var validPayload = await GoogleJsonWebSignature.ValidateAsync(idToken);
            var email = validPayload.Email;

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded) return null;
            }

            var nuteintIds = new List<long> { 301, 205, 601, 208, 606, 204, 605, 303, 291, 306, 307, 203, 269, 539, 324, 299, 1001, 1006, 1002, 290, 261, 260, 1003, 1004, 1005, 513, 221, 511, 207, 514, 454, 262, 639, 322, 321, 326, 421, 334, 312, 507, 268, 325, 610, 611, 696, 612, 625, 652, 697, 613, 626, 673, 662, 653, 687, 614, 617, 674, 663, 859, 618, 670, 675, 669, 619, 851, 685, 627, 615, 628, 672, 689, 852, 853, 620, 855, 629, 857, 624, 630, 858, 631, 621, 654, 671, 607, 608, 609, 645, 646, 693, 695, 313, 417, 431, 435, 432, 212, 287, 515, 211, 516, 512, 521, 503, 213, 504, 338, 337, 505, 214, 506, 304, 428, 315, 406, 573, 578, 257, 664, 676, 856, 665, 666, 305, 410, 508, 636, 517, 319, 405, 317, 518, 641, 209, 638, 210, 263, 404, 502, 323, 341, 343, 342, 501, 509, 510, 318, 320, 418, 415, 401, 328, 430, 429, 255, 309, 344, 345, 346, 347 };

            var nutrients = nuteintIds.Select(id =>
            {
                return new NutrientTracking()
                {
                    NutrientId = id,
                    UserId = user.Id,
                    TargetAmount = 0,
                    IsActive = false
                };
            }).ToList();

            await _dbContext.NutrientTrackings.AddRangeAsync(nutrients);
            await _dbContext.SaveChangesAsync();

            var token = await CreateJWTToken(user, new() { "User" });

            return token;
        }
    }
}
