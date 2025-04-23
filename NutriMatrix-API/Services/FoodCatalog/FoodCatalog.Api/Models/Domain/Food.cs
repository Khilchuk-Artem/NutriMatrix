namespace FoodCatalog.Api.Models.Domain
{
    public class Food:IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
        public bool IsDeleted { get; set; }

        public IEnumerable<Measure> Measures { get; set; }
        public IEnumerable<FoodNutrientIn100g> FoodNutrients { get; set; }
    }
}
