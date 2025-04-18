using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BuildingBlocks.Nutrionix.Models
{
    public class Metadata
    {
        [JsonPropertyName("is_raw_food")]
        public bool IsRawFood { get; set; }
    }
}
