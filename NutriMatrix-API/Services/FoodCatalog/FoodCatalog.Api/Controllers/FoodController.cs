using FoodCatalog.Api.Data.Context;
using FoodCatalog.Api.Features.Foods.Commands;
using FoodCatalog.Api.Features.Foods.Queries;
using FoodCatalog.Api.Features.Foodss.Queries;
using FoodCatalog.Api.Models.Domain;
using FoodCatalog.Api.Models.Dto;
using FoodCatalog.Api.Models.Redis;
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

        public FoodController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id:long}", Name = "GetFoodById")]
        public async Task<IActionResult> Get(long id, [FromQuery] long[]? includeNutrientIds)
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
        public class CreateFoodDto
        {
            public string Name { get; set; }
            public string Photo { get; set; }
            public string? Barcode { get; set; }
            public List<CreateMeasureDto> Measures { get; set; }
            public List<CreateFoodNutrientIn100gDto> Nutrients { get; set; }
        }

        public class CreateMeasureDto
        {
            public string Name { get; set; }
            public float WeightInGrams { get; set; }
        }

        public class CreateFoodNutrientIn100gDto
        {
            public long NutrientId { get; set; }
            public float Amount { get; set; }
        }

    }
}
