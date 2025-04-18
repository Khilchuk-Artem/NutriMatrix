using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BuildingBlocks.Nutrionix.Models
{
    public class Photo
    {
        [JsonPropertyName("thumb")]
        public string Thumb { get; set; }

        [JsonPropertyName("highres")]
        public string Highres { get; set; }

        [JsonPropertyName("is_user_uploaded")]
        public bool IsUserUploaded { get; set; }
    }
}
