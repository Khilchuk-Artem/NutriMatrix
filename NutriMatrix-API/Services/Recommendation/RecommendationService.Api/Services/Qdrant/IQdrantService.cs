
using Qdrant.Client.Grpc;

namespace RecommendationService.Api.Services.Qdrant
{
    public interface IQdrantService
    {
        Task CreateCollectionAsync();
        Task DeleteAllRecordsAsync();
        Task<List<int>> FindKNearestNeighborsAsync(int k, IEnumerable<float> rawSearchVector, string? category = null, List<string>? includeIngredientIds = null, List<string>? excludeIngredientIds = null);
        Task<UpdateStatus> InsertRecipeVectorAsync(int id, IEnumerable<float> rawVector, string category, List<string> ingredientIds);
    }
}
