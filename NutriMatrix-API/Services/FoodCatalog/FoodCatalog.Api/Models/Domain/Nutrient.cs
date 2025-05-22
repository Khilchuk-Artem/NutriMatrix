namespace FoodCatalog.Api.Models.Domain
{
    public class Nutrient:IEntity
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public bool IsDeleted { get; set; }
    }
}
