using Auth.API.Data;
using Auth.API.Models.DTO;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auth.API.Features.Commands
{
    public class UpdateUserSummaryCommand : IRequest<UserSummaryDTO>
    {
        public UpdateUserDTO UpdateUserDTO { get; set; }
        public string UserId { get; set; }
    }

    public class UpdateUserSummaryCommandHandler : IRequestHandler<UpdateUserSummaryCommand, UserSummaryDTO>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AuthDbContext _dbContext;

        public UpdateUserSummaryCommandHandler(UserManager<IdentityUser> userManager, AuthDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<UserSummaryDTO> Handle(UpdateUserSummaryCommand request, CancellationToken cancellationToken)
        {
            var dto = request.UpdateUserDTO;
            if (dto.Name == null) return null;

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user != null)
            {
                user.UserName = dto.Name;
                var identityResult = await _userManager.UpdateAsync(user);
                if (identityResult.Succeeded)
                {
                    foreach (var nutrientUpdate in dto.UpdateNutrients)
                    {
                        var nutrient = await _dbContext.NutrientTrackings.FindAsync(nutrientUpdate.Id);
                        if (nutrient != null && nutrient.UserId == request.UserId)
                        {
                            nutrient.IsActive = nutrientUpdate.IsActive;
                            nutrient.TargetAmount = nutrientUpdate.TargetAmount;
                        }
                    }
                    await _dbContext.SaveChangesAsync();

                    var nutrients = await _dbContext.NutrientTrackings.Where(r => r.UserId == request.UserId).ToArrayAsync();
                    var updatedSummary = new UserSummaryDTO
                    {
                        Id = request.UserId,
                        Name = user.UserName,
                        Email = user.Email,
                        Roles = (await _userManager.GetRolesAsync(user)).ToArray(),
                        NutrientsToTrack = nutrients
                    };
                    return updatedSummary;
                }
            }
            return null;
        }
    }
}
