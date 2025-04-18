using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BuildingBlocks.Nutrionix.Models
{
    public class AltMeasure
    {
        [JsonPropertyName("serving_weight")]
        public double ServingWeight { get; set; }

        [JsonPropertyName("measure")]
        public string Measure { get; set; }

        [JsonPropertyName("seq")]
        public int? Seq { get; set; }

        [JsonPropertyName("qty")]
        public double Qty { get; set; }
    }
}
