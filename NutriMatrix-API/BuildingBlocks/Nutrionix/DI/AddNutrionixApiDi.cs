using BuildingBlocks.Nutrionix.Refit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Nutrionix.DI
{
    public static class AddNutrionixApiDi
    {
        public static IServiceCollection AddNutrionixApi(this IServiceCollection services)
        {
            services.AddTransient<NutritionApiAuthHandler>();

            services
                .AddRefitClient<INutritionApi>()
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri("https://trackapi.nutritionix.com");
                })
                .AddHttpMessageHandler<NutritionApiAuthHandler>();

            return services;
        }
    }
}
