using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecommendationService.Api.Data;
using RecommendationService.Api.Models;
using RecommendationService.Api.Models.Redis;
using Redis.OM.Searching;

namespace RecommendationService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipeController : ControllerBase
    {
        private readonly RecipeDbContext _dbContext;

        public RecipeController(RecipeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("shortcuts/{id:long}")]
        public async Task<IActionResult> GetShortcut(long id, [FromQuery] string? nutrientIds = null)
        {
            long[]? nutrientIdsArray = ParseNutrientIds(nutrientIds);

            var recipe = await _dbContext.Recipes
                .Include(r => r.NutrientsPerTotalServings)
                .Include(r => r.Measures)
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

            if (recipe == null)
                return NotFound();

            var dto = ProjectToDto(recipe, nutrientIdsArray);
            return Ok(dto);
        }

        [HttpGet("shortcuts")]
        public async Task<IActionResult> GetShortcuts(
            [FromQuery] string? category = null,
            [FromQuery] string? query = null,
            [FromQuery] string? nutrientIds = null,
            int page = 1,
            int pageSize = 10)
        {
            long[]? nutrientIdsArray = ParseNutrientIds(nutrientIds);

            var queryable = _dbContext.Recipes
                .Include(r => r.NutrientsPerTotalServings)
                .Include(r => r.Measures)
                .Where(r => !r.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(category))
                queryable = queryable.Where(r => r.Category == category);

            if (!string.IsNullOrWhiteSpace(query))
                queryable = queryable.Where(r => r.Title.Contains(query));

            var results = await queryable
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = results.Select(r => ProjectToDto(r, nutrientIdsArray)).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Parses a comma-separated string of nutrient IDs into a long array.
        /// Returns null if the input is null or empty.
        /// Invalid entries are ignored.
        /// </summary>
        private static long[]? ParseNutrientIds(string? nutrientIds)
        {
            if (string.IsNullOrWhiteSpace(nutrientIds))
                return null;

            var parts = nutrientIds.Split(',', System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries);
            var list = new List<long>();

            foreach (var part in parts)
            {
                if (long.TryParse(part, out var val))
                    list.Add(val);
            }

            return list.Count > 0 ? list.ToArray() : null;
        }

        private static RecipeShortcutDto ProjectToDto(Recipe r, long[]? nutrientIds)
        {
            var filteredNutrients = nutrientIds?.Length > 0
                ? r.NutrientsPerTotalServings
                    .Where(n => nutrientIds.Contains(n.NutrientId))
                    .Select(n => new NutrientAmountDto { NutrientId = n.NutrientId, Amount = n.Amount })
                    .ToList()
                : r.NutrientsPerTotalServings
                    .Select(n => new NutrientAmountDto { NutrientId = n.NutrientId, Amount = n.Amount })
                    .ToList();

            var ingredients = r.Measures.Select(m => new IngredientMeasureDto
            {
                Amount = m.Amount,
                FoodId = m.FoodId,
                MeasureId = m.Id,
            }).ToList();

            return new RecipeShortcutDto
            {
                Id = r.Id,
                Title = r.Title,
                RecipeId = r.Id,
                Servings = r.Servings ?? 0,
                Category = r.Category,
                Ingredients = ingredients,
                Nutrients = filteredNutrients
            };
        }

        // DTO classes

        public class RecipeShortcutDto
        {
            public long Id { get; set; }
            public string Title { get; set; }
            public long RecipeId { get; set; }
            public float Servings { get; set; }
            public string Category { get; set; }
            public List<IngredientMeasureDto> Ingredients { get; set; }
            public List<NutrientAmountDto> Nutrients { get; set; }
        }

        public class IngredientMeasureDto
        {
            public float Amount { get; set; }
            public long FoodId { get; set; }
            public long MeasureId { get; set; }
        }

        public class NutrientAmountDto
        {
            public int NutrientId { get; set; }
            public float Amount { get; set; }
        }
    }
}
