using Auth.Application.DTO;
using Auth.Domain.Entities;
using Auth.Persistance.Specifications;
using Auth.Domain.Contracts;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auth.Application.Features.UserSummary.Commands
{
    public class UpdateUserSummaryCommand : IRequest<UserSummaryDTO>
    {
        public UpdateUserDTO UpdateUserDTO { get; set; }
        public string UserId { get; set; }
    }

    public class UpdateUserSummaryCommandHandler : IRequestHandler<UpdateUserSummaryCommand, UserSummaryDTO>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IRepository<NutrientTracking> _nutrientRepository;

        public UpdateUserSummaryCommandHandler(
            UserManager<IdentityUser> userManager,
            IRepository<NutrientTracking> nutrientRepository)
        {
            _userManager = userManager;
            _nutrientRepository = nutrientRepository;
        }

        public async Task<UserSummaryDTO> Handle(UpdateUserSummaryCommand request, CancellationToken cancellationToken)
        {
            var dto = request.UpdateUserDTO;
            if (dto.Name == null) return null;

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null) return null;

            user.UserName = dto.Name;
            var identityResult = await _userManager.UpdateAsync(user);
            if (!identityResult.Succeeded) return null;

            foreach (var nutrientUpdate in dto.UpdateNutrients)
            {
                var nutrient = await _nutrientRepository.Get(nutrientUpdate.Id);
                if (nutrient != null && nutrient.UserId == request.UserId)
                {
                    nutrient.IsActive = nutrientUpdate.IsActive;
                    nutrient.TargetAmount = nutrientUpdate.TargetAmount;
                    await _nutrientRepository.Update(nutrient);
                }
            }

            var spec = new NutrientTrackingsByUserIdSpecification(request.UserId);
            var nutrients = await _nutrientRepository.GetAll(spec, 1, int.MaxValue);

            return new UserSummaryDTO
            {
                Id = request.UserId,
                Name = user.UserName,
                Email = user.Email,
                Roles = (await _userManager.GetRolesAsync(user)).ToArray(),
                NutrientsToTrack = nutrients.ToArray()
            };
        }
    }
}
