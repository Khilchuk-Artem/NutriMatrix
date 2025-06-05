using MassTransit;
using BuildingBlocks.Messaging.Events;
using BuildingBlocks.Messaging.Extensions;
using FoodCatalog.Grpc;
using FoodRecords.Api.Data;
using FoodRecords.Api.Data.Repositories;
using FoodRecords.Api.Services.FoodRecords;
using FoodRecords.Api.Services.MealFetcher;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Quartz;
using FoodRecords.Api.Services.TaskSchedulerService;

namespace FoodRecords.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IFoodRecordService, FoodRecordService>();
            builder.Services.AddDbContext<FoodRecordsDbContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            //builder.Services.AddMessageBroker(builder.Configuration, Assembly.GetExecutingAssembly());
            //builder.Services.AddMassTransitHostedService();
            // Add this after AddQuartz
            builder.Services.AddSingleton(provider =>
            {
                var factory = provider.GetRequiredService<ISchedulerFactory>();
                var scheduler = factory.GetScheduler().Result;
                return scheduler;
            });
            builder.Services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();
            });
            builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
            builder.Services.AddScoped<ITaskSchedulerService, TaskSchedulerService>();

            builder.Services.AddMassTransit(x =>
            {

                x.UsingRabbitMq((context, configurator) =>
                {
                    configurator.Host(new Uri(builder.Configuration["MessageBroker:Host"]!), host =>
                    {
                        host.Username(builder.Configuration["MessageBroker:UserName"]);
                        host.Password(builder.Configuration["MessageBroker:Password"]);
                    });

                    configurator.ConfigureEndpoints(context);
                });
                x.AddRequestClient<GetMealByIdRequest>();

            });
            builder.Services.AddMassTransitHostedService();

            builder.Services.AddScoped<IMealFetcher, MealFetcher>();
            /*builder.Services.AddGrpcClient<FoodService.FoodServiceClient>(options =>
            {
                options.Address = new Uri(builder.Configuration["GrpcSettings:FoodServiceUri"]!);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };

                return handler;
            });*/


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            using (var scope = app.Services.CreateScope())
            {
                DatabaseHelper.CreateDatabaseIfNotExists("foodrecordsdb");
                var db = scope.ServiceProvider.GetRequiredService<FoodRecordsDbContext>();
                db.Database.Migrate();
            }

            app.UseCors(options =>
            {
                options.AllowAnyHeader();
                options.AllowAnyOrigin();
                options.AllowAnyMethod();
            });
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
