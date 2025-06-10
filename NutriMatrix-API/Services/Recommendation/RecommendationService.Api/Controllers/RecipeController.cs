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
            [FromQuery] string? includeIngredients = null,
            [FromQuery] string? excludeIngredients = null,
            int page = 1,
            int pageSize = 10)
        {
            long[]? include = ParseNutrientIds(includeIngredients);
            long[]? exclude = ParseNutrientIds(excludeIngredients);
            long[]? nutrientIdsArray = ParseNutrientIds(nutrientIds);

            var queryable = _dbContext.Recipes
                .Include(r => r.NutrientsPerTotalServings)
                .Include(r => r.Measures)
                .Where(r=>r.Measures.Count!=0)
                .Where(r => !r.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(category))
                queryable = queryable.Where(r => r.Category == category);

            if (!string.IsNullOrWhiteSpace(query))
                queryable = queryable.Where(r => r.Title.Contains(query));

            if (include is { Length: > 0 })
            {
                queryable = queryable.Where(r =>
                    include.All(ingId => r.Measures.Any(m => m.FoodId == ingId)));
            }


            if (exclude is { Length: > 0 })
                queryable = queryable.Where(r => r.Measures.All(m => !exclude.Contains(m.FoodId)));

            var results = await queryable
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = results.Select(r => ProjectToDto(r, nutrientIdsArray)).ToList();

            return Ok(dtos);
        }
        [HttpPost]
        public async Task<IActionResult> CreateRecipe([FromBody] CreateRecipeDto dto)
        {
            var recipe = new Recipe
            {
                Title = dto.Title,
                Category = dto.Category,
                Description = dto.Description,
                Directions = dto.Directions,
                PhotoUrl = dto.PhotoUrl,
                Servings = dto.Servings,
                IsDeleted = false,
                Measures = dto.Measures.Select(m => new RecipeMeasure
                {
                    MeasureId = m.MeasureId,
                    FoodId = m.FoodId,
                    Amount = m.Amount
                }).ToList(),
                NutrientsPerTotalServings = dto.Nutrients.Select(n => new NutrientAmount
                {
                    NutrientId = n.NutrientId,
                    Amount = n.Amount
                }).ToList()
            };

            _dbContext.Recipes.Add(recipe);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetShortcut), new { id = recipe.Id }, new { recipe.Id });
        }
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteRecipe(long id)
        {
            var recipe = await _dbContext.Recipes.FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
            if (recipe == null)
                return NotFound();

            recipe.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
            return NoContent();
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
        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateRecipe(long id, [FromBody] UpdateRecipeDto dto)
        {
            var recipe = await _dbContext.Recipes
                .Include(r => r.Measures)
                .Include(r => r.NutrientsPerTotalServings)
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

            if (recipe == null)
                return NotFound();

            recipe.Title = dto.Title;
            recipe.Category = dto.Category;
            recipe.Description = dto.Description;
            recipe.Directions = dto.Directions;
            recipe.PhotoUrl = dto.PhotoUrl;
            recipe.Servings = dto.Servings;

            _dbContext.RecipeMeasure.RemoveRange(recipe.Measures);
            recipe.Measures = dto.Measures.Select(m => new RecipeMeasure
            {
                MeasureId = m.MeasureId,
                FoodId = m.FoodId,
                Amount = m.Amount,
                RecipeId = recipe.Id
            }).ToList();

            _dbContext.NutrientAmounts.RemoveRange(recipe.NutrientsPerTotalServings);
            recipe.NutrientsPerTotalServings = dto.Nutrients.Select(n => new NutrientAmount
            {
                NutrientId = n.NutrientId,
                Amount = n.Amount,
                RecipeId = recipe.Id
            }).ToList();

            await _dbContext.SaveChangesAsync();
            return NoContent();
        }
        // GET: api/Recipe/5
        // GET: api/Recipe/5
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetRecipe(
            long id,
            [FromQuery] string? nutrientIds = null           // ← new optional param
        )
        {
            // 1. parse
            long[]? nutrientIdsArray = ParseNutrientIds(nutrientIds);

            // 2. load
            var recipe = await _dbContext.Recipes
                .Include(r => r.Measures)
                .Include(r => r.NutrientsPerTotalServings)
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

            if (recipe == null) return NotFound();

            // 3. map
            var dto = new FullRecipeDto
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Category = recipe.Category,
                Servings = recipe.Servings ?? 0,
                Description = recipe.Description,
                Directions = recipe.Directions,
                PhotoUrl = recipe.PhotoUrl,
                Ingredients = recipe.Measures
                    .Select(m => new IngredientMeasureDto
                    {
                        FoodId = m.FoodId,
                        MeasureId = m.MeasureId,
                        Amount = m.Amount
                    }).ToList(),

                // apply filter if any
                Nutrients = (nutrientIdsArray?.Length > 0
                    ? recipe.NutrientsPerTotalServings
                        .Where(n => nutrientIdsArray.Contains(n.NutrientId))
                    : recipe.NutrientsPerTotalServings
                  )
                  .Select(n => new NutrientAmountDto
                  {
                      NutrientId = n.NutrientId,
                      Amount = n.Amount
                  }).ToList()
            };

            return Ok(dto);
        }

        // add this DTO class below in the controller (or in a shared Models namespace)
        public class FullRecipeDto
        {
            public long Id { get; set; }
            public string Title { get; set; }
            public string Category { get; set; }
            public float Servings { get; set; }
            public string Description { get; set; }
            public string Directions { get; set; }
            public string PhotoUrl { get; set; }
            public List<IngredientMeasureDto> Ingredients { get; set; }
            public List<NutrientAmountDto> Nutrients { get; set; }
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
                MeasureId = m.MeasureId,
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
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories([FromQuery] int? minRecipeCount = null)
        {
            var query = _dbContext.Recipes
                .Where(r => !r.IsDeleted)
                .GroupBy(r => r.Category)
                .Select(g => new { Category = g.Key, RecipeCount = g.Count() });

            if (minRecipeCount.HasValue)
            {
                query = query.Where(c => c.RecipeCount >= minRecipeCount.Value);
            }

            var categories = await query
                .OrderBy(c => c.Category)
                .Select(c => c.Category)
                .ToListAsync();

            return Ok(categories);
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
        public class UpdateRecipeDto
        {
            public string Title { get; set; }
            public string Category { get; set; }
            public float? Servings { get; set; }
            public string Description { get; set; }
            public string Directions { get; set; }
            public string PhotoUrl { get; set; }
            public List<IngredientMeasureDto> Measures { get; set; }
            public List<NutrientAmountDto> Nutrients { get; set; }
        }
        public class CreateRecipeDto
        {
            public string Title { get; set; }
            public string Category { get; set; }
            public float? Servings { get; set; }
            public string Description { get; set; }
            public string Directions { get; set; }
            public string PhotoUrl { get; set; }
            public List<IngredientMeasureDto> Measures { get; set; }
            public List<NutrientAmountDto> Nutrients { get; set; }
        }

    }
}
