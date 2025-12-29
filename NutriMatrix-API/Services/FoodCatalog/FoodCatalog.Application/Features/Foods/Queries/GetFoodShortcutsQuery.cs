using FoodCatalog.Application.Dto;
using FoodCatalog.Domain.Contracts;
using FoodCatalog.Domain.Entities;
using FoodCatalog.Persistance.Context;
using FoodCatalog.Persistance.Redis;
using FoodCatalog.Persistance.Specifications.Foods;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Redis.OM.Searching;

namespace FoodCatalog.Api.Features.Foods.Queries
{
    public class GetFoodShortcutsQuery : IRequest<IEnumerable<FoodShortcutDTO>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public long[]? IncludeNutrientIds { get; set; }
        public string? SearchQuery { get; set; }
    }

    public class GetFoodShortcutsQueryHandler : IRequestHandler<GetFoodShortcutsQuery, IEnumerable<FoodShortcutDTO>>
    {
        private readonly RedisCollection<FoodRedis> _foodCollection;

        public GetFoodShortcutsQueryHandler(RedisCollection<FoodRedis> foodCollection)
        {
            _foodCollection = foodCollection;
        }

        public async Task<IEnumerable<FoodShortcutDTO>> Handle(GetFoodShortcutsQuery request, CancellationToken cancellationToken)
        {
            var query = _foodCollection.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchQuery))
            {
                var loweredSearchQuery = request.SearchQuery.ToLower();
                query = query.Where(f => f.Name.Contains(loweredSearchQuery));
            }

            var foods = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            foreach (var f in foods)
            {
                if (f.FoodNutrients == null)
                {
                    f.FoodNutrients = new List<FoodNutrientIn100g>();
                }
                else if (request.IncludeNutrientIds != null)
                {
                    f.FoodNutrients = f.FoodNutrients
                        .Where(fn => request.IncludeNutrientIds.Contains(fn.NutrientId))
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

            return res;
        }
    }
}
