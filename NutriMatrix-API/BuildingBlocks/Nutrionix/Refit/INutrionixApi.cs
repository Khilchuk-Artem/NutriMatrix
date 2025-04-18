using BuildingBlocks.Nutrionix.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BuildingBlocks.Nutrionix.Refit
{
    public class NutritionQueryRequest
    {
        public string Query { get; set; }
    }
    public interface INutrionixApi
    {
        [Post("/v2/natural/nutrients")]
        Task<List<Food>> GetNutritionFromNaturalInput([Body] NutritionQueryRequest request);
    }
}
