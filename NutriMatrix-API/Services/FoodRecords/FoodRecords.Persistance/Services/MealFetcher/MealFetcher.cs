using BuildingBlocks.Messaging.Events;
using BuildingBlocks.Messaging.Responses;
using MassTransit;

namespace FoodRecords.Persistance.Services.MealFetcher
{

    public class MealFetcher : IMealFetcher
    {
        private readonly IRequestClient<GetMealByIdRequest> _requestClient;

        public MealFetcher(IRequestClient<GetMealByIdRequest> requestClient)
        {
            _requestClient = requestClient;
        }

        public async Task<MealResponseDto?> FetchMealAsync(long mealId)
        {
            var response = await _requestClient.GetResponse<MealResponseDto>(new GetMealByIdRequest() { Id=mealId});
            return response.Message;
        }
    }

}
