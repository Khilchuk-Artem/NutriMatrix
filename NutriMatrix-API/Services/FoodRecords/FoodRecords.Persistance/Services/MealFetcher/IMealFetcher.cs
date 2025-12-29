using BuildingBlocks.Messaging.Responses;

namespace FoodRecords.Persistance.Services.MealFetcher
{
    public interface IMealFetcher
    {
        Task<MealResponseDto?> FetchMealAsync(long mealId);
    }

}
