using Auth.Application.Features.Auth.Queries;
using Auth.Application.Services.EmailService;
using Auth.Application.Settings;
using Auth.Domain.Contracts;
using Auth.Persistance.Context;
using Auth.Persistance.Repository.Implementations;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Auth.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDataProtection();

            builder.Services.AddControllers();

            var connection = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AuthDbContext>(options =>
            {
                options.UseNpgsql(connection, b => b.MigrationsAssembly("Auth.Persistance"));
            });

            builder.Services.Configure<EmailSettings>(
                builder.Configuration.GetSection("EmailSettings"));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GoogleLoginCommand).Assembly));

            builder.Services
                .AddIdentityCore<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("Auth.API")
                .AddEntityFrameworkStores<AuthDbContext>()
                .AddDefaultTokenProviders();
            var serviceAccountJson = builder.Configuration["Firebase:ServiceAccountKey"];
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromJson(serviceAccountJson)
            });

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
                options.SignIn.RequireConfirmedAccount = true;
            });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                DatabaseHelper.CreateDatabaseIfNotExists("AuthDb");
                var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
                //db.Database.Migrate();
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth API V1");
                });
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
