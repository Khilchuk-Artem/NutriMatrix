namespace FoodRecords.Domain.Common
{
    public interface IEntity
    {
        public long Id { get; set; }
        public bool IsDeleted { get; set; }
    }
}
