using Auth.API.Data;
using Auth.API.Models.DTO;
using Auth.API.Models;
using FirebaseAdmin.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth.API.Features.Queries
{
    public class GoogleLoginCommand : IRequest<LoginResponseDTO>
    {
        public string IdToken { get; set; }
    }

    public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommand, LoginResponseDTO>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AuthDbContext _dbContext;
        private readonly IConfiguration _config;
        public GoogleLoginCommandHandler(UserManager<IdentityUser> userManager, AuthDbContext dbContext, IConfiguration config)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _config = config;
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
        public async Task<LoginResponseDTO> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
        {
            var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(request.IdToken);
            string email = decodedToken.Claims.TryGetValue("email", out var emailClaim) ? emailClaim.ToString() : null;
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    return null;
                }

                var nuteintIds = new List<long> { 301, 205, 601, 208, 606, 204, 605, 303, 291, 306, 307, 203, 269, 539, 324, 299, 1001, 1006, 1002, 290, 261, 260, 1003, 1004, 1005, 513, 221, 511, 207, 514, 454, 262, 639, 322, 321, 326, 421, 334, 312, 507, 268, 325, 610, 611, 696, 612, 625, 652, 697, 613, 626, 673, 662, 653, 687, 614, 617, 674, 663, 859, 618, 670, 675, 669, 619, 851, 685, 627, 615, 628, 672, 689, 852, 853, 620, 855, 629, 857, 624, 630, 858, 631, 621, 654, 671, 607, 608, 609, 645, 646, 693, 695, 313, 417, 431, 435, 432, 212, 287, 515, 211, 516, 512, 521, 503, 213, 504, 338, 337, 505, 214, 506, 304, 428, 315, 406, 573, 578, 257, 664, 676, 856, 665, 666, 305, 410, 508, 636, 517, 319, 405, 317, 518, 641, 209, 638, 210, 263, 404, 502, 323, 341, 343, 342, 501, 509, 510, 318, 320, 418, 415, 401, 328, 430, 429, 255, 309, 344, 345, 346, 347 };
                var nutrients = nuteintIds.Select(id => new NutrientTracking
                {
                    NutrientId = id,
                    UserId = user.Id,
                    TargetAmount = 0,
                    IsActive = false
                }).ToList();
                await _dbContext.NutrientTrackings.AddRangeAsync(nutrients);
                await _dbContext.SaveChangesAsync();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var nutrientTrackings = await _dbContext.NutrientTrackings.Where(r => r.UserId == user.Id).ToArrayAsync();
            var token = await CreateJWTToken(user, roles.ToList());

            return new LoginResponseDTO
            {
                Name = user.UserName,
                Email = user.Email,
                UserId = user.Id,
                Roles = roles.ToArray(),
                Token = token,
                NutrientTrackings = nutrientTrackings
            };
        }
    }
}
