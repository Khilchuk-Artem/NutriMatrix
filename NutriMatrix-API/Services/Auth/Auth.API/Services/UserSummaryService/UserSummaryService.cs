using Auth.API.Data;
using Auth.API.Models.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Auth.API.Services.UserSummaryService
{
    public class UserSummaryService:IUserSummaryService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AuthDbContext _dbContext;
        public UserSummaryService(UserManager<IdentityUser> userManager, AuthDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<UserSummaryDTO> GetUserSummary(string id)
        {
            var user = await _userManager.FindByIdAsync(id);


            if (user != null)
            {
                var n = await _dbContext.NutrientTrackings.Where(r => r.UserId == id).ToArrayAsync();
                var summary = new UserSummaryDTO()
                {
                    Id = id,
                    Name = user.UserName,
                    Email = user.Email,
                    Roles = (await _userManager.GetRolesAsync(user)).ToArray(),
                    NutrientsToTrack = n
                };
                return summary;
            }

            return null;
        }

        public async Task<UserSummaryDTO> UpdateUserSummaryById(UpdateUserDTO dto, string userId)
        {
            if (dto.Name == null) return null;
            IdentityUser user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                user.UserName = dto.Name;

                IdentityResult identityResult = await _userManager.UpdateAsync(user);


                if (identityResult.Succeeded)
                {
                    foreach(var a in dto.UpdateNutrients)
                    {
                        var nut = await _dbContext.NutrientTrackings.FindAsync(a.Id);

                        nut.IsActive = a.IsActive;
                        nut.TargetAmount = a.TargetAmount;

                        await _dbContext.SaveChangesAsync();
                    }


                    var n = await _dbContext.NutrientTrackings.Where(r => r.UserId == userId).ToArrayAsync();

                    

                    var updatedSummary = new UserSummaryDTO()
                    {
                        Id = userId,
                        Name = user.UserName,
                        Email = user.Email,
                        Roles = (await _userManager.GetRolesAsync(user)).ToArray(),
                        NutrientsToTrack = n
                    };
                    return updatedSummary;
                };
            }
            return null;
        }

    }
}
