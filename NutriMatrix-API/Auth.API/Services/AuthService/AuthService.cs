using Auth.API.Models.DTO;
using Auth.API.Services.EmailService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
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
        public AuthService(UserManager<IdentityUser> userManager, IConfiguration config, IEmailService emailService)
        {
            _userManager = userManager;
            _config = config;
            _emailService = emailService;
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
                    if (roles != null)
                    {
                        var response = new LoginResponseDTO()
                        {
                            Name = user.UserName,
                            Email = user.Email,
                            UserId = user.Id,
                            Roles = roles.ToArray(),
                            Token = await CreateJWTToken(user, roles.ToList())
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
                            var token = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
                            var confirmationLink = $"{_config["ClientUrl"]}/confirm-email?token={HttpUtility.UrlEncode(token)}&email={HttpUtility.UrlEncode(registerUserDTO.Email)}";

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

            var resetLink = $"{_config["ClientUrl"]}/reset-password?token={HttpUtility.UrlEncode(token)}&email={HttpUtility.UrlEncode(email)}";

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
    }
}
