using FoodRecords.Api.Models.Domain;
using FoodRecords.Api.Models.Dto;

namespace FoodRecords.Api.Services.FoodRecords
{
    public interface IFoodRecordService
    {
        public IEnumerable<FoodRecord> GetAll();
        public Task<FoodRecordDto> GetAsync(Guid id);
        public Task<FoodRecord> AddAsync(AddFoodRecordDto dto);
        public Task<FoodRecord> DeleteAsync(Guid id);
        public Task<FoodRecord> UpdateAsync(Guid id, UpdateFoodRecordDto dto);
    }
}
