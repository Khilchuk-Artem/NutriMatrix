using Auth.API.Data;
using Auth.API.Models.DTO;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auth.API.Features.Queries
{
    public class GetUserSummaryQuery : IRequest<UserSummaryDTO>
    {
        public string UserId { get; set; }
    }

    public class GetUserSummaryQueryHandler : IRequestHandler<GetUserSummaryQuery, UserSummaryDTO>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AuthDbContext _dbContext;

        public GetUserSummaryQueryHandler(UserManager<IdentityUser> userManager, AuthDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<UserSummaryDTO> Handle(GetUserSummaryQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user != null)
            {
                var nutrients = await _dbContext.NutrientTrackings.Where(r => r.UserId == request.UserId).ToArrayAsync();
                var summary = new UserSummaryDTO
                {
                    Id = request.UserId,
                    Name = user.UserName,
                    Email = user.Email,
                    Roles = (await _userManager.GetRolesAsync(user)).ToArray(),
                    NutrientsToTrack = nutrients
                };
                return summary;
            }
            return null;
        }
    }
}
