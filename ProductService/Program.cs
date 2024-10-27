using Microsoft.EntityFrameworkCore;
using ProductService.Models; 
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Serilog;

namespace ProductService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Log.Information("Starting ProductService web host");
                var builder = WebApplication.CreateBuilder(args);
                IConfiguration config = builder.Configuration;

                

                // Настройка SQL Server
                //      builder.Services.AddDbContext<ProductContext>(options =>
                //         options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


                builder.Services.AddDbContext<ProductContext>(options =>
                   options.UseInMemoryDatabase("ProductDatabase"));
               

                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowAll", builder =>
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader());
                });

                // Добавление сервисов в контейнер
                builder.Services.AddControllers();

                // Настройка Swagger
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProductService", Version = "v1" });

                    // Добавление схемы безопасности для JWT
                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Введите 'Bearer' [пробел] и ваш токен в поле ниже. \r\n\r\nПример: \"Bearer 12345abcdef\"",
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey
                    });

                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}
                        }
                    });
                });

                var app = builder.Build();

                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();
                app.UseCors("AllowAll");
                app.UseAuthentication();
                app.UseAuthorization();
                app.MapControllers();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
