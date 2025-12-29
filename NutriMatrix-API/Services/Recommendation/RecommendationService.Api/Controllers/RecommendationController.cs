using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecommendationService.Api.Services.RecommendationService;
using RecommendationService.Application.Features.Recommendations.Queries;
using RecommendationService.Application.Models.Dto;

namespace RecommendationService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecommendationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RecommendationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("get-recommendation")]
        public async Task<IActionResult> Recommendation(RecommendationRequestDto dto)
        {
            var query = new GetRecommendationQuery { Dto = dto };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
