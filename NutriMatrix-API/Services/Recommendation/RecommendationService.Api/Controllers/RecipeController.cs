using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecommendationService.Application.Features.Recipes.Commands;
using RecommendationService.Application.Features.Recipes.Queries;
using RecommendationService.Application.Models.Dto;
using Redis.OM.Searching;

namespace RecommendationService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RecipeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("shortcuts/{id:long}")]
        public async Task<IActionResult> GetShortcut(long id, [FromQuery] string? nutrientIds)
        {
            var query = new GetRecipeShortcutByIdQuery { Id = id, NutrientIds = nutrientIds };
            var result = await _mediator.Send(query);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet("shortcuts")]
        public async Task<IActionResult> GetShortcuts(
            [FromQuery] string? category = null,
            [FromQuery] string? query = null,
            [FromQuery] string? nutrientIds = null,
            [FromQuery] string? includeIngredients = null,
            [FromQuery] string? excludeIngredients = null,
            int page = 1,
            int pageSize = 10)
        {
            var request = new GetRecipeShortcutsQuery
            {
                Category = category,
                Query = query,
                NutrientIds = nutrientIds,
                IncludeIngredients = includeIngredients,
                ExcludeIngredients = excludeIngredients,
                Page = page,
                PageSize = pageSize
            };
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecipe([FromBody] CreateRecipeDto dto)
        {
            var command = new CreateRecipeCommand { Dto = dto };
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetShortcut), new { id }, new { id });
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteRecipe(long id)
        {
            try
            {
                var command = new DeleteRecipeCommand { Id = id };
                await _mediator.Send(command);
                return NoContent();
            }
            catch (Exception ex) when (ex.Message.Contains("not found"))
            {
                return NotFound();
            }
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateRecipe(long id, [FromBody] UpdateRecipeDto dto)
        {
            try
            {
                var command = new UpdateRecipeCommand { Id = id, Dto = dto };
                await _mediator.Send(command);
                return NoContent();
            }
            catch (Exception ex) when (ex.Message.Contains("not found"))
            {
                return NotFound();
            }
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetRecipe(long id, [FromQuery] string? nutrientIds)
        {
            var query = new GetFullRecipeByIdQuery { Id = id, NutrientIds = nutrientIds };
            var result = await _mediator.Send(query);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories([FromQuery] int? minRecipeCount)
        {
            var query = new GetRecipeCategoriesQuery { MinRecipeCount = minRecipeCount };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
