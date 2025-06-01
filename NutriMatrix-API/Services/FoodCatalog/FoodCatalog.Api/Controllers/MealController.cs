using FoodCatalog.Api.Data.Context;
using FoodCatalog.Api.Models.Domain;
using FoodCatalog.Api.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodCatalog.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MealController : ControllerBase
    {
        private readonly FoodCatalogDbContext _dbContext;

        public MealController(FoodCatalogDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetMeals(string userId, int pageNumber = 1, int pageSize = 20, string? searchQuery = null)
        {
            var query = _dbContext.Meals
                .Where(m => !m.IsDeleted)
                .Where(m => m.AddedBy == userId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(m => m.Name.ToLower().Contains(searchQuery.ToLower()));
            }

            var meals = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new MealDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    AddedBy = m.AddedBy,
                    TotalServings = m.TotalServings,
                    FoodMeals = m.FoodMeals
                        .Select(fm => new FoodMealDto
                        {
                            MeasureId = fm.MeasureId,
                            Quantity = fm.Quantity
                        })
                        .ToList()
                })
                .ToListAsync();

            return Ok(meals);
        }

        [HttpGet("{id:long}", Name = "GetMealById")]
        public async Task<IActionResult> GetMealById(long id)
        {
            var meal = await _dbContext.Meals
                .Where(m => m.Id == id && !m.IsDeleted)
                .Select(m => new MealDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    AddedBy = m.AddedBy,
                    TotalServings = m.TotalServings,
                    FoodMeals = m.FoodMeals
                        .Select(fm => new FoodMealDto
                        {
                            Id = fm.Id,
                            MeasureId = fm.MeasureId,
                            Quantity = fm.Quantity
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (meal == null)
            {
                return NotFound();
            }

            return Ok(meal);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMeal([FromBody] CreateMealDto createMealDto)
        {
            foreach(var a in createMealDto.FoodMeals)
            {
                var tmp = await _dbContext.Measures.FirstOrDefaultAsync(ms => ms.Id == a.MeasureId);

                var kek = new Meal();
            }

            var meal = new Meal
            {
                Name = createMealDto.Name,
                AddedBy = createMealDto.AddedBy,
                TotalServings = createMealDto.TotalServings,
                IsDeleted = false,
                FoodMeals = createMealDto.FoodMeals
                    .Select(fm => new FoodMeal
                    {
                        MeasureId = fm.MeasureId,
                        Quantity = fm.Quantity
                    })
                    .ToList()
            };

            _dbContext.Meals.Add(meal);
            await _dbContext.SaveChangesAsync();

            var mealDto = new MealDto
            {
                Id = meal.Id,
                Name = meal.Name,
                AddedBy = meal.AddedBy,
                TotalServings = meal.TotalServings,
                FoodMeals = meal.FoodMeals
                    .Select(fm => new FoodMealDto
                    {
                        MeasureId = fm.MeasureId,
                        Quantity = fm.Quantity
                    })
                    .ToList()
            };

            return CreatedAtRoute("GetMealById", new { id = meal.Id }, mealDto);
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateMeal(long id, [FromBody] UpdateMealDto updateMealDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var meal = await _dbContext.Meals
                .Include(m => m.FoodMeals)
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);

            if (meal == null)
            {
                return NotFound();
            }

            meal.Name = updateMealDto.Name;
            meal.AddedBy = updateMealDto.AddedBy;
            meal.TotalServings = updateMealDto.TotalServings;

            _dbContext.FoodMeals.RemoveRange(meal.FoodMeals);
            meal.FoodMeals = updateMealDto.FoodMeals
                .Select(fm => new FoodMeal
                {
                    MeasureId = fm.MeasureId,
                    Quantity = fm.Quantity
                })
                .ToList();

            await _dbContext.SaveChangesAsync();

            return Ok(meal);
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteMeal(long id)
        {
            var meal = await _dbContext.Meals
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);

            if (meal == null)
            {
                return NotFound();
            }

            meal.IsDeleted = true;
            await _dbContext.SaveChangesAsync();

            return Ok(meal);
        }
    }
}
