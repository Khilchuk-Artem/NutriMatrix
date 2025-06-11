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
                Barcode = food.Barcode,
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
                        Barcode = cachedFood.Barcode,
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
                Barcode = food.Barcode,
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
        [HttpPost]
        public async Task<IActionResult> CreateFood([FromBody] CreateFoodDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest("Food name cannot be empty.");
            }

            // Retrieve current maximum IDs from the database
            long currentMaxFoodId = await _dbCont.Foods.AnyAsync() ? await _dbCont.Foods.MaxAsync(f => f.Id) : 0;
            long currentMaxMeasureId = await _dbCont.Measures.AnyAsync() ? await _dbCont.Measures.MaxAsync(m => m.Id) : 0;
            long currentMaxNutrientId = await _dbCont.FoodNutrientIn100Gs.AnyAsync() ? await _dbCont.FoodNutrientIn100Gs.MaxAsync(n => n.Id) : 0;

            // Create new food with incremented ID
            currentMaxFoodId++;
            var food = new Food
            {
                Id = currentMaxFoodId,
                Name = dto.Name,
                Photo = dto.Photo,
                Barcode = dto.Barcode,
                IsDeleted = false,
                Measures = new List<Measure>(),
                FoodNutrients = new List<FoodNutrientIn100g>()
            };

            foreach(var measureDto in dto.Measures ?? new List<CreateMeasureDto>())
            {
                currentMaxMeasureId++;
                var measure = new Measure
                {
                    Id = currentMaxMeasureId,
                    Name = measureDto.Name,
                    WeightInGrams = measureDto.WeightInGrams,
                    FoodId = food.Id,
                    IsDeleted = false
                };
                food.Measures.Add(measure);
            }

            foreach (var nutrientDto in dto.Nutrients ?? new List<CreateFoodNutrientIn100gDto>())
            {
                currentMaxNutrientId++;
                var nutrient = new FoodNutrientIn100g
                {
                    Id = currentMaxNutrientId,
                    NutrientId = nutrientDto.NutrientId,
                    Amount = nutrientDto.Amount,
                    FoodId = food.Id,
                    IsDeleted = false
                };
                food.FoodNutrients.Add(nutrient);
            }

            // Add to database and save
            _dbCont.Foods.Add(food);
            await _dbCont.SaveChangesAsync();

            // Cache in Redis
            var foodRedis = new FoodRedis
            {
                Id = food.Id,
                Name = food.Name,
                Photo = food.Photo,
                Barcode = food.Barcode,
                FoodNutrients = food.FoodNutrients.Select(n => new FoodNutrientIn100g
                {
                    NutrientId = n.NutrientId,
                    Amount = n.Amount,
                    IsDeleted = n.IsDeleted
                }).ToList()
            };
            await _foodCollection.InsertAsync(foodRedis);

            foreach (var measure in food.Measures)
            {
                var measureRedis = new MeasureRedis
                {
                    Id = measure.Id,
                    Name = measure.Name,
                    WeightInGrams = measure.WeightInGrams,
                    FoodId = food.Id
                };
                await _measureCollection.InsertAsync(measureRedis);
            }

            return CreatedAtAction(nameof(Get), new { id = food.Id }, food);
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
