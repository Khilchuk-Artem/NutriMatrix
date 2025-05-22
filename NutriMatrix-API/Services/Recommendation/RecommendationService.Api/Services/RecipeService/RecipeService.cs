using RecommendationService.Api.Models.Dto;

namespace RecommendationService.Api.Services.RecipeService
{
    public class RecipeService : IRecipeService
    {
        public RecipeShortcutDto CreateRecipe(CreateRecipeWithIngredientIdsDto dto)
        {
            throw new NotImplementedException();
        }

        public RecipeDraftDto CreateRecipeFromRawData(CreateRecipeWithCleanIngredients rawData)
        {
            throw new NotImplementedException();
        }

        public RecipeShortcutDto GetShortcutById(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
