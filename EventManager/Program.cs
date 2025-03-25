
using Application.Interface;
using Application.Services;
using Application.Validation;
using Domain.Interfaces;
using EventManager.Controllers;
using FluentValidation.AspNetCore;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Presentation.Filters;
using System.Text;

namespace EventManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            if (!builder.Environment.IsEnvironment("Testing"))
            {
                builder = WebApplication.CreateBuilder(new WebApplicationOptions
                {
                    ContentRootPath = Directory.GetCurrentDirectory(),
                    WebRootPath = "wwwroot" // Это папка в проекте Presentation
                });
            }
            


            // Загружаем конфигурацию
            builder.Configuration.AddEnvironmentVariables();
            var configuration = builder.Configuration;

            // Получаем настройки JWT
            var jwtSettings = configuration.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            // Регистрируем контроллеры:
            // Вызов AddControllers с добавлением ApplicationPart, содержащей контроллеры из внешней сборки.
            // Для примера ExternalProject.Controllers.ExternalController – класс, расположенный во внешнем проекте.
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<EventExceptionFilter>(); // Глобальный фильтр, если требуется
            })
            .AddApplicationPart(typeof(EventsController).Assembly) // подключаем внешнюю сборку
            .AddFluentValidation(options =>
            {
                options.RegisterValidatorsFromAssemblyContaining<UserRequestValidation>();
            });

            // Остальные настройки (Swagger, FormOptions, аутентификация, EF Core и т.д.)
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 10 * 1024 * 1024;
            });

            // Настраиваем аутентификацию с JWT
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],

                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            // Регистрируем EF Core в зависимости от среды
            if (builder.Environment.IsEnvironment("Testing"))
            {
                builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase("InMemoryDbForTesting"));
            }
            else
            {
                builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
            }

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    policy => policy.WithOrigins("http://localhost:3000")
                                    .AllowAnyHeader()
                                    .AllowAnyMethod());
            });

            // Регистрация других сервисов
            builder.Services.AddScoped<IEventService, EventService>();
            builder.Services.AddScoped<IEventRepository, EventRepository>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

            var app = builder.Build();

            // Конфигурация middleware
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHttpsRedirection();
            app.UseCors("AllowSpecificOrigin");

            app.UseRouting();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            if (!builder.Environment.IsEnvironment("Testing"))
            {
                using (var scope = app.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    dbContext.Database.Migrate();
                }
            }

            app.Run();
        }
    }
}
