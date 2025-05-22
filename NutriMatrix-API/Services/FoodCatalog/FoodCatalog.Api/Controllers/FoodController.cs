using FoodCatalog.Api.Data.Context;
using FoodCatalog.Api.Models.Domain;
using FoodCatalog.Api.Models.Dto;
using FoodCatalog.Api.Models.Redis;
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
        private readonly RedisCollection<FoodRedis> _foodCollection;
        private readonly RedisCollection<MeasureRedis> _measureCollection;
        private readonly FoodCatalogDbContext _dbCont;

        public FoodController(RedisCollection<FoodRedis> foodCollection, RedisCollection<MeasureRedis> measureCollection, FoodCatalogDbContext dbCont)
        {
            _foodCollection = foodCollection;
            _measureCollection = measureCollection;
            _dbCont = dbCont;
        }
        [HttpGet("{id:long}", Name = "GetFoodById")]
        public async Task<IActionResult> Get(long id, [FromQuery]long[]? includeNuntrientIds)
        {
            var food = await _foodCollection.FirstOrDefaultAsync(f => f.Id==id);
            if (includeNuntrientIds != null)
            {
                food.FoodNutrients = food.FoodNutrients.Where(fn => includeNuntrientIds.Contains(fn.NutrientId)).ToList();
            }
            if (food == null) return NotFound();

            var measures = await _measureCollection.Where(m => m.FoodId == id).ToListAsync();

            var res = new FoodDTO
            {
                Id = food.Id,
                Name = food.Name,
                Photo = food.Photo,
                FoodNutrients = food.FoodNutrients?
                .Where(n => !n.IsDeleted)
                .Select(n => new FoodNutrientIn100gDto
                {
                    NutrientId = n.NutrientId,
                    Amount = n.Amount
                })
                .ToList(),
                Measures = measures?
                .Select(m => new MeasureDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    WeightInGrams = m.WeightInGrams
                })
                .ToList()
            };

            return Ok(res);
        }
        [HttpGet(Name = "GetFoodShortcuts")]
        public async Task<IActionResult> GetShortcuts(
            int pageNumber = 1,
            int pageSize = 5,
            [FromQuery] long[]? includeNuntrientIds = null,
            string? searchQuery = null)
        {
            var query = _foodCollection.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var loweredSearchQuery = searchQuery.ToLower();
                query = query.Where(f => f.Name.Contains(searchQuery));

            }

            var foods = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            foreach (var f in foods)
            {
                if (f.FoodNutrients == null)
                {
                    f.FoodNutrients = new List<FoodNutrientIn100g>();
                }
                else if (includeNuntrientIds != null)
                {
                    f.FoodNutrients = f.FoodNutrients
                        .Where(fn => includeNuntrientIds.Contains(fn.NutrientId))
                        .ToList();
                }
            }

            var res = foods.Select(f => new FoodShortcutDTO
            {
                Id = f.Id,
                Name = f.Name,
                Nutrients = f.FoodNutrients
                    ?.Where(n => !n.IsDeleted)
                    .Select(n => new FoodNutrientIn100gDto
                    {
                        NutrientId = n.NutrientId,
                        Amount = n.Amount
                    })
                    .ToList() ?? new List<FoodNutrientIn100gDto>() 
            });

            return Ok(res);
        }
        [HttpGet("by-barcode/{barcode}", Name = "GetFoodByBarcode")]
        public async Task<IActionResult> GetByBarcode(string barcode, [FromQuery] long[]? includeNutrientIds = null)
        {
            // 1. Try from Redis
            var cachedFood = await _foodCollection.FirstOrDefaultAsync(f => f.Barcode == barcode);
            if (cachedFood != null)
            {
                var m1 = _measureCollection.Where(m => m.FoodId == cachedFood.Id).ToList();
                var m2 = new List<Measure>(); 
                if (m1.Count == 0) m2 = await _dbCont.Measures.Where(m => m.FoodId == cachedFood.Id).ToListAsync();
                if (m1.Count != 0)
                {
                    var res12 = new FoodDTO
                    {
                        Id = cachedFood.Id,
                        Name = cachedFood.Name,
                        Photo = cachedFood.Photo,
                        FoodNutrients = cachedFood.FoodNutrients
                                        .Where(n => !n.IsDeleted)
                                        .Select(n => new FoodNutrientIn100gDto
                                        {
                                            NutrientId = n.NutrientId,
                                            Amount = n.Amount
                                        })
                                        .ToList(),
                        Measures = m1
                                        .Select(m => new MeasureDto
                                        {
                                            Id = m.Id,
                                            Name = m.Name,
                                            WeightInGrams = m.WeightInGrams
                                        }).ToList()

                    };

                    return Ok(res12);
                }
                else
                {
                    var res12 = new FoodDTO
                    {
                        Id = cachedFood.Id,
                        Name = cachedFood.Name,
                        Photo = cachedFood.Photo,
                        FoodNutrients = cachedFood.FoodNutrients
                                        .Where(n => !n.IsDeleted)
                                        .Select(n => new FoodNutrientIn100gDto
                                        {
                                            NutrientId = n.NutrientId,
                                            Amount = n.Amount
                                        })
                                        .ToList(),
                        Measures = m2
                                        .Select(m => new MeasureDto
                                        {
                                            Id = m.Id,
                                            Name = m.Name,
                                            WeightInGrams = m.WeightInGrams
                                        }).ToList()

                    };

                    return Ok(res12);
                }
            }

            // 2. Fallback to DB
            var food = await _foodCollection.FirstOrDefaultAsync(f => f.Barcode == barcode);
            if (food == null)
                return NotFound();

            if (food.FoodNutrients == null)
                food.FoodNutrients = new List<FoodNutrientIn100g>();
            else if (includeNutrientIds != null)
                food.FoodNutrients = food.FoodNutrients
                    .Where(fn => includeNutrientIds.Contains(fn.NutrientId))
                    .ToList();

            // 3. Store to Redis for next time
            var foodRedis = new FoodRedis
            {
                Id = food.Id,
                Name = food.Name,
                Photo = food.Photo,
                Barcode = food.Barcode,
                FoodNutrients = food.FoodNutrients.Where(n => !n.IsDeleted)
            };
            //await _redisService.SaveAsync(foodRedis);

            // 4. Return DTO
            var measures = await _measureCollection.Where(m => m.FoodId == food.Id).ToListAsync();
            var res = new FoodDTO
            {
                Id = food.Id,
                Name = food.Name,
                Photo = food.Photo,
                FoodNutrients = food.FoodNutrients
                    .Where(n => !n.IsDeleted)
                    .Select(n => new FoodNutrientIn100gDto
                    {
                        NutrientId = n.NutrientId,
                        Amount = n.Amount
                    })
                    .ToList(),
                Measures = measures
                    .Select(m => new MeasureDto
                    {
                        Id = m.Id,
                        Name = m.Name,
                        WeightInGrams = m.WeightInGrams
                    })
                    .ToList()
            };

            return Ok(res);
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
