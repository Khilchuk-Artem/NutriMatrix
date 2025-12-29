using MediatR;
using RecommendationService.Api.Services.RecommendationService;
using RecommendationService.Application.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationService.Application.Features.Recommendations.Queries
{
    public class GetRecommendationQuery : IRequest<RecommendationResponseDto>
    {
        public RecommendationRequestDto Dto { get; set; }
    }

    public class GetRecommendationQueryHandler : IRequestHandler<GetRecommendationQuery, RecommendationResponseDto>
    {
        private readonly IRecipeRecommendationService _recipeRecommendationService;

        public GetRecommendationQueryHandler(IRecipeRecommendationService recipeRecommendationService)
        {
            _recipeRecommendationService = recipeRecommendationService;
        }

        public async Task<RecommendationResponseDto> Handle(GetRecommendationQuery request, CancellationToken cancellationToken)
        {
            return await _recipeRecommendationService.GetRecommendationAsync(request.Dto);
        }
    }
}
