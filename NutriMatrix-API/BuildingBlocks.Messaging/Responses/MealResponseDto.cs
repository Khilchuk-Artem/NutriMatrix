using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Messaging.Responses
{
    public class MealResponseDto
    {
        public MealResponseDto(long id, string name, string addedBy, float totalServings, List<FoodMealResponseDto> foodMeals)
        {
            Id = id;
            Name = name;
            AddedBy = addedBy;
            TotalServings = totalServings;
            FoodMeals = foodMeals;
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string AddedBy { get; set; }
        public float TotalServings { get; set; }
        public List<FoodMealResponseDto> FoodMeals { get; set; }
    }
}
