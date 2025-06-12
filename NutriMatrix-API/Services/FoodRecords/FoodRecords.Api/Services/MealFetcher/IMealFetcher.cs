using BuildingBlocks.Messaging.Responses;

namespace FoodRecords.Api.Services.MealFetcher
{
    public interface IMealFetcher
    {
        Task<MealResponseDto?> FetchMealAsync(long mealId);
    }

}
