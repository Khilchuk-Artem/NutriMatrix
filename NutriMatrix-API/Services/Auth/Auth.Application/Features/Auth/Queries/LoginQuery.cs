using Auth.Application.DTO;
using Auth.Domain.Entities;
using Auth.Persistance.Specifications;
using Auth.Domain.Contracts;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Runtime.Intrinsics.X86;

namespace Auth.Application.Features.Auth.Queries
{
    public class LoginQuery : IRequest<LoginResponseDTO>
    {
        public LoginUserDTO LoginUserDTO { get; set; }
    }

    public class LoginQueryHandler : IRequestHandler<LoginQuery, LoginResponseDTO>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IRepository<NutrientTracking> _nutrientRepository;
        private readonly IConfiguration _config;

        public LoginQueryHandler(
            UserManager<IdentityUser> userManager,
            IRepository<NutrientTracking> nutrientRepository,
            IConfiguration config)
        {
            _userManager = userManager;
            _nutrientRepository = nutrientRepository;
            _config = config;
        }

        public async Task<LoginResponseDTO> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.LoginUserDTO.Email);

            if (user != null)
            {
                var passwordCheck = await _userManager.CheckPasswordAsync(user, request.LoginUserDTO.Password);
                if (passwordCheck)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var spec = new NutrientTrackingsByUserIdSpecification(user.Id);
                    var nutrientTrackings = await _nutrientRepository.GetAll(spec, 1, int.MaxValue);

                    return new LoginResponseDTO
                    {
                        Name = user.UserName,
                        Email = user.Email,
                        UserId = user.Id,
                        Roles = roles.ToArray(),
                        Token = await CreateJWTToken(user, roles.ToList()),
                        NutrientTrackings = nutrientTrackings.ToArray()
                    };
                }
            }

            return null;
        }

        private async Task<string> CreateJWTToken(IdentityUser user, List<string> roles)
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
    }
}
