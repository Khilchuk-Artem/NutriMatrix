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
    public interface INutritionApi
    {
        [Post("/v2/natural/nutrients")]
        Task<List<Food>> GetNutritionFromNaturalInput([Body] string query);
    }
}
