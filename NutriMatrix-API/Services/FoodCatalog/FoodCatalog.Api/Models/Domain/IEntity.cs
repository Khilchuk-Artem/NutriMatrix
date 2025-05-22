namespace FoodCatalog.Api.Models.Domain
{
    public interface IEntity
    {
        long Id { get; set; }
        bool IsDeleted { get; set; }
    }
}
