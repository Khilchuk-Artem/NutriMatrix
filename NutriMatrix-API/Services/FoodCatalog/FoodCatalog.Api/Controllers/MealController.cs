using FoodCatalog.Api.Features.Meals.Commands;
using FoodCatalog.Api.Features.Meals.Queries;
using FoodCatalog.Application.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodCatalog.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MealController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MealController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetMeals(string userId, int pageNumber = 1, int pageSize = 20, string? searchQuery = null)
        {
            var query = new GetMealsQuery { UserId = userId, PageNumber = pageNumber, PageSize = pageSize, SearchQuery = searchQuery };
            var meals = await _mediator.Send(query);
            return Ok(meals);
        }

        [HttpGet("{id:long}", Name = "GetMealById")]
        public async Task<IActionResult> GetMealById(long id)
        {
            var query = new GetMealByIdQuery { Id = id };
            var meal = await _mediator.Send(query);
            if (meal == null)
            {
                return NotFound();
            }
            return Ok(meal);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMeal([FromBody] CreateMealDto createMealDto)
        {
            try
            {
                var command = new CreateMealCommand { CreateMealDto = createMealDto };
                var mealDto = await _mediator.Send(command);
                return CreatedAtRoute("GetMealById", new { id = mealDto.Id }, mealDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateMeal(long id, [FromBody] UpdateMealDto updateMealDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = new UpdateMealCommand { Id = id, UpdateMealDto = updateMealDto };
            var updatedMeal = await _mediator.Send(command);
            if (updatedMeal == null)
            {
                return NotFound();
            }
            return Ok(updatedMeal);
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteMeal(long id)
        {
            var command = new DeleteMealCommand { Id = id };
            var deletedMeal = await _mediator.Send(command);
            if (deletedMeal == null)
            {
                return NotFound();
            }
            return Ok(deletedMeal);
        }
    }
}
