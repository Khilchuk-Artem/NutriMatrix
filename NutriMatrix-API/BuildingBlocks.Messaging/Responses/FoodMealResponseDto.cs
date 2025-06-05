using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Messaging.Responses
{
    public class FoodMealResponseDto
    {
        public FoodMealResponseDto(long id, long measureId, float quantity, string measureName, float weightInGrams)
        {
            Id = id;
            MeasureId = measureId;
            Quantity = quantity;
            MeasureName = measureName;
            WeightInGrams = weightInGrams;
        }

        public long Id { get; set; }
        public long MeasureId { get; set; }
        public float Quantity { get; set; }
        public string MeasureName { get; set; }
        public float WeightInGrams { get; set; }
    }
}
