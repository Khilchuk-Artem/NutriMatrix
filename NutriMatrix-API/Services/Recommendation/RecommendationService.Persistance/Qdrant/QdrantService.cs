using Qdrant.Client;
using Qdrant.Client.Grpc;
using System.Net.NetworkInformation;
using static Qdrant.Client.Grpc.Conditions;

namespace RecommendationService.Persistance.Qdrant
{
    public class QdrantService : IQdrantService
    {
        private readonly QdrantClient _qudrantClient;
        private readonly string _collectionName = "recipe_nutrients";
        public QdrantService(QdrantClient qudrantClient)
        {
            _qudrantClient = qudrantClient;
        }
        public async Task CreateCollectionAsync()
        {
            await _qudrantClient.CreateCollectionAsync(_collectionName, new VectorParams
            {
                Size = 161,
                Distance = Distance.Cosine,
                HnswConfig = new HnswConfigDiff
                {
                    M = 16,
                    EfConstruct = 100
                }
            });
        }

        public async Task<UpdateStatus> InsertRecipeVectorAsync(int id, IEnumerable<float> rawVector, string category, List<string> ingredientIds)
        {
            if (rawVector.Count() != 161) throw new ArgumentException("Incorrect vector size");

            var point = new PointStruct()
            {
                Id = (ulong)id,
                Vectors = PerformUnitNormalization(rawVector).ToArray(),
                Payload = {
                    ["recipeId"] = id,
                    ["category"] = category,
                    ["ingredients"] = ingredientIds.ToArray()
                }
            };

            var updateResult = await _qudrantClient.UpsertAsync(_collectionName, new List<PointStruct> { point });

            return updateResult.Status;
        }

        public async Task<List<int>> FindKNearestNeighborsAsync(
            int k,
            IEnumerable<float> rawSearchVector,
            IEnumerable<int>? excludeIds = null,
            string? category = null,
            List<string>? includeIngredientIds = null,
            List<string>? excludeIngredientIds = null)
        {
            Filter filterOptions = null;


            if (category != null)
            {
                filterOptions = MatchKeyword("category", category);
            }
            if (excludeIds != null)
            {
                foreach (var id in excludeIds)
                {
                    if (filterOptions == null) filterOptions = !MatchKeyword("recipeId", id.ToString());
                    else filterOptions &= !MatchKeyword("recipeId", id.ToString());
                }
            }
            if (includeIngredientIds != null)
            {
                foreach (var ing in includeIngredientIds)
                {
                    if (filterOptions == null) filterOptions = MatchKeyword("ingredients", ing);
                    else filterOptions &= MatchKeyword("ingredients", ing);
                }
            }
            if (excludeIngredientIds != null)
            {
                foreach (var ing in excludeIngredientIds)
                {
                    if (filterOptions == null) filterOptions = !MatchKeyword("ingredients", ing);
                    else filterOptions &= !MatchKeyword("ingredients", ing);
                }
            }

            var queryVector = PerformUnitNormalization(rawSearchVector).ToArray();

            var results = await _qudrantClient.SearchAsync(_collectionName, queryVector, limit: (ulong)k, filter: filterOptions);

            return results.Select(r => Convert.ToInt32(r.Id.Num)).ToList();
        }

        private IEnumerable<float> PerformUnitNormalization(IEnumerable<float> rawVector)
        {
            var norm = MathF.Sqrt(rawVector.Sum(x => x * x));

            return rawVector.Select(x => x / norm);
        }

        public async Task DeleteAllRecordsAsync()
        {
            await _qudrantClient.DeleteCollectionAsync(_collectionName);
        }
    }
}
