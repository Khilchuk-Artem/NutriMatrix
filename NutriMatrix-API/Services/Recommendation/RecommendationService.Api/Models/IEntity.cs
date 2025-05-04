namespace RecommendationService.Api.Models
{
    public interface IEntity
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
    }
}
