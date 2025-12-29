using FoodCatalog.Api.Features.Foods.Commands;
using FoodCatalog.Api.Features.Foods.Queries;
using FoodCatalog.Api.Features.Foodss.Queries;
using FoodCatalog.Application.Dto;
using FoodCatalog.Domain.Entities;
using FoodCatalog.Persistance.Context;
using FoodCatalog.Persistance.Redis;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Redis.OM.Searching;
using System.Numerics;

namespace FoodCatalog.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FoodController : Controller
    {
        private readonly IMediator _mediator;
        private readonly RedisCollection<FoodRedis> _foodRedisCollection;
        private readonly RedisCollection<MeasureRedis> _measureRedisCollection;
        private readonly FoodCatalogDbContext _dbContext;
        public FoodController(IMediator mediator, RedisCollection<MeasureRedis> measureRedisCollection, RedisCollection<FoodRedis> foodRedisCollection, FoodCatalogDbContext dbContext)
        {
            _mediator = mediator;
            _measureRedisCollection = measureRedisCollection;
            _foodRedisCollection = foodRedisCollection;
            _dbContext = dbContext;
        }

        [HttpGet("{id:long}", Name = "GetFoodById")]
        public async Task<IActionResult> Get(long id, [FromQuery] long[]? includeNutrientIds = null)
        {
            var query = new GetFoodByIdQuery { Id = id, IncludeNutrientIds = includeNutrientIds };
            var result = await _mediator.Send(query);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet(Name = "GetFoodShortcuts")]
        public async Task<IActionResult> GetShortcuts(
            int pageNumber = 1,
            int pageSize = 5,
            [FromQuery] long[]? includeNutrientIds = null,
            string? searchQuery = null)
        {
            var query = new GetFoodShortcutsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                IncludeNutrientIds = includeNutrientIds,
                SearchQuery = searchQuery
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("by-barcode/{barcode}", Name = "GetFoodByBarcode")]
        public async Task<IActionResult> GetByBarcode(string barcode, [FromQuery] long[]? includeNutrientIds = null)
        {
            var query = new GetFoodByBarcodeQuery { Barcode = barcode, IncludeNutrientIds = includeNutrientIds };
            var result = await _mediator.Send(query);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFood([FromBody] CreateFoodDto dto)
        {
            try
            {
                var command = new CreateFoodCommand { CreateFoodDto = dto };
                var food = await _mediator.Send(command);
                return CreatedAtAction(nameof(Get), new { id = food.Id }, food);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("seeding")]
        public async Task<IActionResult> SeedRedis()
        {
            var foods = await _dbContext.Foods
                .Include(f => f.FoodNutrients)
                .Include(f => f.Measures)
                .ToListAsync();

            foreach (var food in foods)
            {
                var foodRedis = new FoodRedis { Id = food.Id };

                foodRedis.Name = food.Name ?? foodRedis.Name;
                foodRedis.Photo = food.Photo ?? foodRedis.Photo;
                foodRedis.Barcode = food.Barcode;
                foodRedis.FoodNutrients = food.FoodNutrients?
                    .Select(n => new FoodNutrientIn100g
                    {
                        Id = n.Id,
                        FoodId = n.FoodId,
                        NutrientId = n.NutrientId,
                        Amount = n.Amount,
                        IsDeleted = n.IsDeleted
                    }).ToList();

                await _foodRedisCollection.InsertAsync(foodRedis);

                if (food.Measures != null)
                {
                    foreach (var measure in food.Measures)
                    {
                        var measureRedis = new MeasureRedis { Id = measure.Id };

                        measureRedis.Name = measure.Name;
                        measureRedis.WeightInGrams = measure.WeightInGrams;
                        measureRedis.FoodId = food.Id;

                        await _measureRedisCollection.InsertAsync(measureRedis);
                    }
                }
            }
            return Ok();
        }
        private FoodDTO MapFoodRedisToDto(FoodRedis redis, long[]? includeNutrientIds)
        {
            var nutrients = redis.FoodNutrients
                .Where(n => !n.IsDeleted && (includeNutrientIds == null || includeNutrientIds.Contains(n.NutrientId)))
                .Select(n => new FoodNutrientIn100gDto
                {
                    NutrientId = n.NutrientId,
                    Amount = n.Amount
                })
                .ToList();

            return new FoodDTO
            {
                Id = redis.Id,
                Name = redis.Name,
                Photo = redis.Photo,
                FoodNutrients = nutrients,
                Measures = [],
            };
        }

    }
}
