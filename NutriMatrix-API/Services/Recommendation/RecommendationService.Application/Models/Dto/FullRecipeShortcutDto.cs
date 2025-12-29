using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationService.Application.Models.Dto
{
    public class FullRecipeShortcutDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public long RecipeId { get; set; }
        public float Servings { get; set; }
        public string Category { get; set; }
        public List<IngredientMeasureDto> Ingredients { get; set; }
        public List<NutrientAmountDto> Nutrients { get; set; }
    }
}
