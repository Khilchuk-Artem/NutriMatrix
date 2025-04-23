namespace FoodCatalog.Api.Models.Domain
{
    public interface IEntity
    {
        Guid Id { get; set; }
        bool IsDeleted { get; set; }
    }
}
