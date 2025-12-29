using Auth.Application.DTO;
using Auth.Domain.Entities;
using Auth.Persistance.Specifications;
using Auth.Domain.Contracts;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Auth.Application.Features.UserSummary.Queries
{
    public class GetUserSummaryQuery : IRequest<UserSummaryDTO>
    {
        public string UserId { get; set; }
    }
    public class GetUserSummaryQueryHandler : IRequestHandler<GetUserSummaryQuery, UserSummaryDTO>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IRepository<NutrientTracking> _nutrientTrackingRepository;

        public GetUserSummaryQueryHandler(
            UserManager<IdentityUser> userManager,
            IRepository<NutrientTracking> nutrientTrackingRepository)
        {
            _userManager = userManager;
            _nutrientTrackingRepository = nutrientTrackingRepository;
        }

        public async Task<UserSummaryDTO> Handle(GetUserSummaryQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);
            var spec = new NutrientTrackingsByUserIdSpecification(request.UserId);
            var nutrients = await _nutrientTrackingRepository.GetAll(spec, 1, int.MaxValue);

            return new UserSummaryDTO
            {
                Id = request.UserId,
                Name = user.UserName,
                Email = user.Email,
                Roles = roles.ToArray(),
                NutrientsToTrack = nutrients.ToArray()
            };
        }
    }
}
