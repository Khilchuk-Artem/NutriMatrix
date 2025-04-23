namespace FoodCatalog.Api.Models.Domain
{
    public class Measure:IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int WeightInGrams { get; set; }
        public Guid FoodId { get; set; }
        public bool IsDeleted { get; set; }

        public Food Food { get; set; }
    }
}
