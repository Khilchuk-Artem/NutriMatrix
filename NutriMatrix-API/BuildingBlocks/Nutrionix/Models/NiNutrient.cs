using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BuildingBlocks.Nutrionix.Models
{
    public class NiNutrient
    {
        [JsonPropertyName("attr_id")]
        public int AttrId { get; set; }

        [JsonPropertyName("value")]
        public double Value { get; set; }
    }
}
