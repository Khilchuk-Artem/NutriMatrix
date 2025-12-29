using BuildingBlocks.Nutrionix.Refit;
using FoodCatalog.Application.Dto;
using FoodCatalog.Domain.Entities;
using FoodCatalog.Persistance.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodCatalog.Api.Features.Measures.Queries
{
    public class SearchMeasuresQuery : IRequest<List<SearchMeasureWithFoodDto>>
    {
        public string Query { get; set; }
    }

    public class SearchMeasuresQueryHandler : IRequestHandler<SearchMeasuresQuery, List<SearchMeasureWithFoodDto>>
    {
        private readonly FoodCatalogDbContext _dbContext;
        private readonly INutrionixApi _nutritionixService;

        public SearchMeasuresQueryHandler(FoodCatalogDbContext dbContext, INutrionixApi nutritionixService)
        {
            _dbContext = dbContext;
            _nutritionixService = nutritionixService;
        }

        public async Task<List<SearchMeasureWithFoodDto>> Handle(SearchMeasuresQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Query))
            {
                throw new ArgumentException("Query cannot be empty.");
            }

            var nutritionixResponse = await _nutritionixService.GetNutritionFromNaturalInput(new NutritionQueryRequest { Query = request.Query });
            if (nutritionixResponse?.Foods == null || !nutritionixResponse.Foods.Any())
            {
                return new List<SearchMeasureWithFoodDto>();
            }

            var result = new List<SearchMeasureWithFoodDto>();
            var foodsToAdd = new List<Food>();
            var measuresToAdd = new List<Measure>();

            long currentMaxFoodId = await _dbContext.Foods.AnyAsync() ? await _dbContext.Foods.MaxAsync(f => f.Id) : 0;
            long currentMaxMeasureId = await _dbContext.Measures.AnyAsync() ? await _dbContext.Measures.MaxAsync(m => m.Id) : 0;
            long currentMaxNutrientId = await _dbContext.FoodNutrientIn100Gs.AnyAsync() ? await _dbContext.FoodNutrientIn100Gs.MaxAsync(n => n.Id) : 0;

            var nuteintIds = new List<int> { 301, 205, 601, 208, 606, 204, 605, 303, 291, 306, 307, 203, 269, 539, 324, 299, 1001, 1006, 1002, 290, 261, 260, 1003, 1004, 1005, 513, 221, 511, 207, 514, 454, 262, 639, 322, 321, 326, 421, 334, 312, 507, 268, 325, 610, 611, 696, 612, 625, 652, 697, 613, 626, 673, 662, 653, 687, 614, 617, 674, 663, 859, 618, 670, 675, 669, 619, 851, 685, 627, 615, 628, 672, 689, 852, 853, 620, 855, 629, 857, 624, 630, 858, 631, 621, 654, 671, 607, 608, 609, 645, 646, 693, 695, 313, 417, 431, 435, 432, 212, 287, 515, 211, 516, 512, 521, 503, 213, 504, 338, 337, 505, 214, 506, 304, 428, 315, 406, 573, 578, 257, 664, 676, 856, 665, 666, 305, 410, 508, 636, 517, 319, 405, 317, 518, 641, 209, 638, 210, 263, 404, 502, 323, 341, 343, 342, 501, 509, 510, 318, 320, 418, 415, 401, 328, 430, 429, 255, 309, 344, 345, 346, 347 };

            foreach (var niFood in nutritionixResponse.Foods)
            {
                var food = await _dbContext.Foods
                    .Include(f => f.Measures)
                    .Include(f => f.FoodNutrients)
                    .FirstOrDefaultAsync(f => f.Name.ToLower() == niFood.FoodName.ToLower() && !f.IsDeleted);

                Measure measure;
                var measureName = niFood.ServingUnit;

                if (food == null)
                {
                    currentMaxFoodId++;
                    food = new Food
                    {
                        Id = currentMaxFoodId,
                        Name = niFood.FoodName,
                        Photo = niFood.Photo?.Thumb ?? @"https://d2eawub7utcl6.cloudfront.net/images/nix-apple-grey.png",
                        Barcode = null,
                        IsDeleted = false,
                        Measures = new List<Measure>(),
                        FoodNutrients = new List<FoodNutrientIn100g>()
                    };

                    if (niFood.ServingWeightGrams > 0 && niFood.FullNutrients != null)
                    {
                        var nutrientsFromApi = niFood.FullNutrients?.ToDictionary(n => n.AttrId, n => n.Value) ?? new Dictionary<int, double>();

                        foreach (var nutrientId in nuteintIds)
                        {
                            nutrientsFromApi.TryGetValue(nutrientId, out var apiValue);
                            var amountPer100g = niFood.ServingWeightGrams > 0
                                ? (float)(apiValue / niFood.ServingWeightGrams * 100)
                                : 0f;

                            currentMaxNutrientId++;
                            food.FoodNutrients.Add(new FoodNutrientIn100g
                            {
                                Id = currentMaxNutrientId,
                                NutrientId = nutrientId,
                                Amount = amountPer100g,
                                IsDeleted = false
                            });
                        }
                    }

                    currentMaxMeasureId++;
                    measure = new Measure
                    {
                        Id = currentMaxMeasureId,
                        Name = measureName,
                        WeightInGrams = (float)niFood.ServingWeightGrams,
                        Food = food,
                        IsDeleted = false
                    };
                    food.Measures.Add(measure);
                    measuresToAdd.Add(measure);
                    foodsToAdd.Add(food);
                }
                else
                {
                    measure = food.Measures.FirstOrDefault(m => m.Name.ToLower() == measureName.ToLower() && !m.IsDeleted);
                    if (measure == null)
                    {
                        currentMaxMeasureId++;
                        measure = new Measure
                        {
                            Id = currentMaxMeasureId,
                            Name = measureName,
                            WeightInGrams = (float)niFood.ServingWeightGrams,
                            Food = food,
                            IsDeleted = false
                        };
                        food.Measures.Add(measure);
                        measuresToAdd.Add(measure);
                    }
                }

                if (niFood.AltMeasures != null && niFood.AltMeasures.Any())
                {
                    var existingMeasureNames = food.Measures.Select(m => m.Name.ToLower()).ToHashSet();
                    foreach (var altMeasure in niFood.AltMeasures)
                    {
                        var altMeasureName = altMeasure.Measure.ToLower();
                        if (!existingMeasureNames.Contains(altMeasureName))
                        {
                            currentMaxMeasureId++;
                            var newMeasure = new Measure
                            {
                                Id = currentMaxMeasureId,
                                Name = altMeasure.Measure,
                                WeightInGrams = (float)altMeasure.ServingWeight,
                                Food = food,
                                IsDeleted = false
                            };
                            food.Measures.Add(newMeasure);
                            measuresToAdd.Add(newMeasure);
                            existingMeasureNames.Add(altMeasureName);
                        }
                    }
                }

                var dto = new SearchMeasureWithFoodDto
                {
                    Id = measure.Id,
                    Name = measure.Name,
                    WeightInGrams = measure.WeightInGrams,
                    Quantity = niFood.ServingQty,
                    Food = new FoodDTO
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
                        Measures = food.Measures?
                            .Select(m => new MeasureDto
                            {
                                Id = m.Id,
                                Name = m.Name,
                                WeightInGrams = m.WeightInGrams
                            })
                            .ToList()
                    }
                };

                result.Add(dto);
            }

            if (foodsToAdd.Any())
            {
                _dbContext.Foods.AddRange(foodsToAdd);
            }
            if (measuresToAdd.Any())
            {
                _dbContext.Measures.AddRange(measuresToAdd);
            }
            await _dbContext.SaveChangesAsync();

            return result;
        }
    }
}
