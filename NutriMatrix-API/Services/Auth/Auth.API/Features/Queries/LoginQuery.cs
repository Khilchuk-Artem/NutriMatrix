using Auth.API.Data;
using Auth.API.Models.DTO;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth.API.Features.Queries
{
    public class LoginQuery : IRequest<LoginResponseDTO>
    {
        public LoginUserDTO LoginUserDTO { get; set; }
    }

    public class LoginQueryHandler : IRequestHandler<LoginQuery, LoginResponseDTO>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AuthDbContext _dbContext;
        private readonly IConfiguration _config;

        public LoginQueryHandler(UserManager<IdentityUser> userManager, AuthDbContext dbContext, IConfiguration config)
        {
            _userManager = userManager;
            _dbContext = dbContext;
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
    }
}
