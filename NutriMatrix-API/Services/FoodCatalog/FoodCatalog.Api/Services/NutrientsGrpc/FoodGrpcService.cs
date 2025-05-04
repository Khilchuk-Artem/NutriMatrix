using FoodCatalog.Api.Data.Context;
using FoodCatalog.Api.Models.Domain;
using FoodCatalog.Api.Models.Redis;
using FoodCatalog.Grpc;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Redis.OM.Searching;

namespace FoodCatalog.Api.Services.NutrientsGrpc
{
    public class FoodGrpcService : FoodService.FoodServiceBase
    {
        private readonly IRedisCollection<MeasureRedis> _measureCollection;
        private readonly IRedisCollection<FoodRedis> _foodCollection;

        public FoodGrpcService(IRedisCollection<MeasureRedis> measureCollection, IRedisCollection<FoodRedis> foodCollection)
        {
            _measureCollection = measureCollection;
            _foodCollection = foodCollection;
        }


        public async Task<FoodMeasureInfoResponse> GetMeasureInfo(GetMeasureRequest request, ServerCallContext context)
        {
            var measureId = Guid.Parse(request.MeasureId);

            var measure = await _measureCollection.FirstOrDefaultAsync();

            if (measure == null)
                throw new RpcException(new Status(StatusCode.NotFound, "Measure not found"));

            var food = await _foodCollection.FirstOrDefaultAsync(m => m.Id == measure.FoodId);

            var response = new FoodMeasureInfoResponse
            {
                Id = measure.Id.ToString(),
                FoodName = food.Name,
                MeasureName = measure.Name,
                WeightInG = measure.WeightInGrams,
                NutrientsIn100G = { food.FoodNutrients.Select(fn => new NutrientAmountDto
                    {
                        Id = fn.Nutrient.Id.ToString(),
                        Name = fn.Nutrient.Name,
                        Amount = fn.Amount
                    })}
            };

            return response;
        }
        public override async Task<FoodMeasureInfoResponse> GetMeasureInfoByName(GetMeasureByNamesRequest request, ServerCallContext context)
        {
            var food = await _foodCollection.FirstOrDefaultAsync(f => f.Name == request.FoodName);
            if (food == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Food '{request.FoodName}' not found."));

            var measure = await _measureCollection.FirstOrDefaultAsync(m =>
                m.FoodId == food.Id && m.Name == request.MeasureName);

            if (measure == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Measure '{request.MeasureName}' for food '{request.FoodName}' not found."));

            var response = new FoodMeasureInfoResponse
            {
                Id = measure.Id.ToString(),
                FoodName = food.Name,
                MeasureName = measure.Name,
                WeightInG = measure.WeightInGrams,
                NutrientsIn100G = {
                    food.FoodNutrients.Select(fn => new NutrientAmountDto
                    {
                        Id = fn.Nutrient.Id.ToString(),
                        Name = fn.Nutrient.Name,
                        Amount = fn.Amount
                    })
                }
            };

            return response;
        }

    }

}
