using FoodCatalog.Grpc;
using FoodRecords.Api.Data;
using FoodRecords.Api.Data.Repositories;
using FoodRecords.Api.Models.Domain;
using FoodRecords.Api.Models.Dto;

namespace FoodRecords.Api.Services.FoodRecords
{
    public class FoodRecordService : IFoodRecordService
    {
        private readonly IRepository<FoodRecord> _repository;
        //private readonly FoodService.FoodServiceClient _foodService;
        private readonly FoodRecordsDbContext _context;

        public FoodRecordService(IRepository<FoodRecord> repository, FoodRecordsDbContext context)
        {
            _repository = repository;
            _context = context;
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

        public async Task<FoodRecord> DeleteAsync(long id)
        {
            var res = await _repository.Delete(id);

            return res;
        }

        public async Task<FoodRecordDto> GetAsync(long id)
        {
            var record = await _repository.Get(id);

            if (record == null) return null;

            //var foodInfo = await _foodService.GetMeasureInfoAsync(new() { MeasureId = record.FoodMeasureId.ToString() });

            //if (foodInfo == null) return null;

            var dto = new FoodRecordDto
            {
                RecordId = record.Id,
                FoodMeasureId = record.FoodMeasureId,
                Amount = record.Amount,
            };

            return dto;
        }

        public IEnumerable<FoodRecord> GetAll(
            string userId,
            bool sortByDateAsc = true,
            DateTime? dateFrom = null,
            DateTime? dateTo = null)
        {
            var query = _context.FoodRecords
                .Where(r => r.UserId == userId && !r.IsDeleted);

            if (dateFrom.HasValue)
                query = query.Where(r => r.DateEaten >= dateFrom.Value);

            if (dateTo.HasValue)
                query = query.Where(r => r.DateEaten <= dateTo.Value);

            query = sortByDateAsc
                ? query.OrderBy(r => r.DateEaten)
                : query.OrderByDescending(r => r.DateEaten);

            return query.ToList();
        }


        public async Task<FoodRecord> UpdateAsync(long id, UpdateFoodRecordDto dto)
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
