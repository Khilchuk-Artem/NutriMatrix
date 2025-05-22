namespace FoodCatalog.Api.Models.Domain
{
    public class Measure:IEntity
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public float WeightInGrams { get; set; }
        public long FoodId { get; set; }
        public bool IsDeleted { get; set; }

        public Food Food { get; set; }
    }
}
