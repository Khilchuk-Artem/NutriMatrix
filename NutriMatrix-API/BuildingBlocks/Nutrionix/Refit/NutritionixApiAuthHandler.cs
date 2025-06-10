using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Nutrionix.Refit
{
    public class NutritionApiAuthHandler : DelegatingHandler
    {
        private readonly IConfiguration _config;

        public NutritionApiAuthHandler(IConfiguration config)
        {
            _config = config;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var value = _config["Nutrionix:AppId"];
            request.Headers.Add("x-app-id", _config["Nutrionix:AppId"]);
            request.Headers.Add("x-app-key", _config["Nutrionix:AppKey"]);

            return base.SendAsync(request, cancellationToken);
        }
    }
}
