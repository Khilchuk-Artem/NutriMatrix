using MassTransit;
using BuildingBlocks.Messaging.Events;
using BuildingBlocks.Messaging.Extensions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Quartz;
using Microsoft.EntityFrameworkCore.Diagnostics;
using FoodRecords.Application.Features.MealRecords.Queries;
using FoodRecords.Domain.Contracts;
using FoodRecords.Persistance.Repository.Implementations;
using FoodRecords.Persistance.Data;
using FoodRecords.Persistance.Services.MealFetcher;

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
            builder.Services.AddDbContext<FoodRecordsDbContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("FoodRecords.Persistance"));
            });

            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetMealRecordByIdQuery).Assembly));

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

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            using (var scope = app.Services.CreateScope())
            {
                DatabaseHelper.CreateDatabaseIfNotExists("foodrecordsdb");
                var db = scope.ServiceProvider.GetRequiredService<FoodRecordsDbContext>();
                //db.Database.Migrate();
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
