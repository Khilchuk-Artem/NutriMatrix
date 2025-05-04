using FoodCatalog.Grpc;
using FoodRecords.Api.Data.Repositories;
using FoodRecords.Api.Models.Domain;
using FoodRecords.Api.Models.Dto;

namespace FoodRecords.Api.Services.FoodRecords
{
    public class FoodRecordService : IFoodRecordService
    {
        private readonly IRepository<FoodRecord> _repository;
        private readonly FoodService.FoodServiceClient _foodService;

        public FoodRecordService(IRepository<FoodRecord> repository, FoodService.FoodServiceClient foodService)
        {
            _repository = repository;
            _foodService = foodService;
        }

        public async Task<FoodRecord> AddAsync(AddFoodRecordDto dto)
        {
            var newRecord = new FoodRecord
            {
                DateEaten = dto.DateEaten,
                UserId = dto.UserId,
                FoodMeasureId = dto.FoodMeasureId,
                Amount = dto.Amount,
                IsDeleted = false
            };

            var result = await _repository.Add(newRecord);

            return result;
        }

        public async Task<FoodRecord> DeleteAsync(Guid id)
        {
            var res = await _repository.Delete(id);

            return res;
        }

        public async Task<FoodRecordDto> GetAsync(Guid id)
        {
            var record = await _repository.Get(id);

            if (record == null) return null;

            var foodInfo = await _foodService.GetMeasureInfoAsync(new() { MeasureId = record.FoodMeasureId.ToString() });

            if (foodInfo == null) return null;

            var totalWeightInGrams = foodInfo.WeightInG * record.Amount;

            var nutrients = foodInfo.NutrientsIn100G
                .Select(n => new Models.Dto.NutrientAmountDto
                {
                    NutrientId = Guid.Parse(n.Id),
                    Name = n.Name,
                    Amount = n.Amount * (totalWeightInGrams / 100f)
                }).ToList();

            var dto = new FoodRecordDto
            {
                RecordId = record.Id,
                FoodMeasureId = record.FoodMeasureId,
                FoodName = foodInfo.FoodName,
                MeasureName = foodInfo.MeasureName,
                MeasureWeightInGrams = foodInfo.WeightInG,
                Amount = record.Amount,
                Nutrients = nutrients
            };

            return dto;
        }

        public IEnumerable<FoodRecord> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<FoodRecord> UpdateAsync(Guid id, UpdateFoodRecordDto dto)
        {
            var entity = await _repository.Get(id);

            if (entity == null) return null;

            entity.Amount = dto.Amount;
            entity.FoodMeasureId = dto.FoodMeasureId;

            var res = await _repository.Update(entity);

            return res;
        }
    }
}
