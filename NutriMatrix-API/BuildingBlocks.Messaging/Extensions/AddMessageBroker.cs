using BuildingBlocks.Messaging.Events;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Messaging.Extensions
{
    public static class Extentions
    {
        public static IServiceCollection AddMessageBroker(
            this IServiceCollection services,
            IConfiguration configuration,
            Assembly? assembly = null,
            IEnumerable<Type>? requestClientTypes = null)
        {
            services.AddMassTransit(config =>
            {
                config.SetKebabCaseEndpointNameFormatter();

                if (assembly != null)
                {
                    config.AddConsumers(assembly);
                }

                /*f (requestClientTypes != null)
                {
                    foreach (var requestClientType in requestClientTypes)
                    {
                        config.AddRequestClient(requestClientType);
                    }
                }*/
                config.AddRequestClient<GetMealByIdRequest>();


                config.UsingRabbitMq((context, configurator) =>
                {
                    configurator.Host(new Uri(configuration["MessageBroker:Host"]!), host =>
                    {
                        host.Username(configuration["MessageBroker:UserName"]);
                        host.Password(configuration["MessageBroker:Password"]);
                    });

                    configurator.ConfigureEndpoints(context);
                });
            });

            return services;
        }

    }
}
