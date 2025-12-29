using MediatR;
using RecommendationService.Application.Models.Dto;
using RecommendationService.Domain.Contracts;
using RecommendationService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationService.Application.Features.Recipes.Commands
{
    public class CreateRecipeCommand : IRequest<long>
    {
        public CreateRecipeDto Dto { get; set; }
    }
    public class CreateRecipeCommandHandler : IRequestHandler<CreateRecipeCommand, long>
    {
        private readonly IRepository<Recipe> _repository;

        public CreateRecipeCommandHandler(IRepository<Recipe> repository)
        {
            _repository = repository;
        }

        public async Task<long> Handle(CreateRecipeCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
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

            var addedRecipe = await _repository.Add(recipe);
            return addedRecipe.Id;
        }
    }
}
