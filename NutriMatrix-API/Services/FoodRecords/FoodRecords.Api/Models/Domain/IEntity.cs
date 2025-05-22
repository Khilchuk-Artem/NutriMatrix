namespace FoodRecords.Api.Models.Domain
{
    public interface IEntity
    {
        public long Id { get; set; }
        public bool IsDeleted { get; set; }
    }
}
