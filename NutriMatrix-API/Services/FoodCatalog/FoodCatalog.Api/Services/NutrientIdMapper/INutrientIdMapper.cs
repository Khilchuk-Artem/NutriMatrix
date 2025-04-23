namespace FoodCatalog.Api.Services.NutrientIdMapper
{
    public interface INutrientIdMapper
    {
        Guid? GetGuidForAttrId(int attrId);
    }
}
