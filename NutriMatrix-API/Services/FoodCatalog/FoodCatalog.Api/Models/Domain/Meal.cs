namespace FoodCatalog.Api.Models.Domain
{
    public class Meal:IEntity
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string AddedBy { get; set; }
        public bool IsDeleted { get; set; }

        public IEnumerable<Food> Foods { get; set; }

    }
}
