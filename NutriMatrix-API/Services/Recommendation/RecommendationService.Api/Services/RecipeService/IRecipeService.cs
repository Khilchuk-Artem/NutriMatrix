using RecommendationService.Api.Data;
using RecommendationService.Api.Models.Dto;

namespace RecommendationService.Api.Services.RecipeService
{
    public interface IRecipeService
    {
        public RecipeShortcutDto GetShortcutById(Guid id);
        public RecipeShortcutDto CreateRecipe(CreateRecipeWithIngredientIdsDto dto);
        public RecipeDraftDto CreateRecipeFromRawData(CreateRecipeWithCleanIngredients rawData);
    }
}
