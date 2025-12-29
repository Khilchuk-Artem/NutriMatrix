using MediatR;
using Microsoft.EntityFrameworkCore;
using RecommendationService.Persistance.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationService.Application.Features.Recipes.Queries
{
    public class GetRecipeCategoriesQuery : IRequest<List<string>>
    {
        public int? MinRecipeCount { get; set; }
    }
    public class GetRecipeCategoriesQueryHandler : IRequestHandler<GetRecipeCategoriesQuery, List<string>>
    {
        private readonly RecipeDbContext _dbContext;

        public GetRecipeCategoriesQueryHandler(RecipeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<string>> Handle(GetRecipeCategoriesQuery request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Recipes
                .Where(r => !r.IsDeleted)
                .GroupBy(r => r.Category)
                .Select(g => new { Category = g.Key, RecipeCount = g.Count() });

            if (request.MinRecipeCount.HasValue)
                query = query.Where(c => c.RecipeCount >= request.MinRecipeCount.Value);

            return await query
                .OrderBy(c => c.Category)
                .Select(c => c.Category)
                .ToListAsync(cancellationToken);
        }
    }
}
