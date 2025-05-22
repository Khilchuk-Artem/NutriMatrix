using FoodRecords.Api.Models.Domain;
using FoodRecords.Api.Models.Dto;

namespace FoodRecords.Api.Services.FoodRecords
{
    public interface IFoodRecordService
    {
        public IEnumerable<FoodRecord> GetAll(string userId,
            bool sortByDateAsc = true,
            DateTime? dateFrom = null,
            DateTime? dateTo = null);
        public Task<FoodRecordDto> GetAsync(long id);
        public Task<FoodRecord> AddAsync(AddFoodRecordDto dto);
        public Task<FoodRecord> DeleteAsync(long id);
        public Task<FoodRecord> UpdateAsync(long id, UpdateFoodRecordDto dto);
    }
}
