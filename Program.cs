using Microsoft.EntityFrameworkCore;
using UserService.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Serilog;


namespace UserService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Настройка Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                Log.Information("Starting web host");
                var builder = WebApplication.CreateBuilder(args);
                IConfiguration config = builder.Configuration;

                // Загрузка настроек JWT из appsettings.json
                  var jwtSettings = builder.Configuration.GetSection("JwtSettings");
                 var key = jwtSettings["Secret"];
                 var issuer = jwtSettings["Issuer"];
                 var audience = jwtSettings["Audience"];

                // Настройка in-memory базы данных
                builder.Services.AddDbContext<UserContext>(opt =>
                    opt.UseInMemoryDatabase("UserDb"));

                // Настройка Identity
                builder.Services.AddIdentity<User, IdentityRole>()
                    .AddEntityFrameworkStores<UserContext>()
                    .AddDefaultTokenProviders();

                // Настройка аутентификации JWT
                builder.Services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {

                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                    };

                    // Логирование событий аутентификации
                    x.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            Log.Warning("Authentication failed: {Message}", context.Exception.Message);
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            Log.Information("Token validated successfully.");
                            return Task.CompletedTask;
                        },
                        OnMessageReceived = context =>
                        {
                            Log.Information("JWT token received: {Token}", context.Request.Headers["Authorization"].ToString());
                            return Task.CompletedTask;
                        }
                    };
                });





                builder.Services.AddAuthorization();
                // Добавление сервисов в контейнер
                builder.Services.AddControllers();


                // Настройка логирования Serilog
                builder.Host.UseSerilog();


               

                

                // Настройка Swagger
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "UserService", Version = "v1" });

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

                // Настройка HTTP request pipeline
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();

                // Middleware для логирования запросов и ответов
                app.Use(async (context, next) =>
                {
                    Log.Information("Received request: {Method} {Path}", context.Request.Method, context.Request.Path);

                    // Логируем тело запроса
                    context.Request.EnableBuffering(); // Позволяет читать тело запроса несколько раз
                    var bodyAsText = await new StreamReader(context.Request.Body).ReadToEndAsync();
                    Log.Information("Request body: {Body}", bodyAsText);

                    // Логируем токен из заголовка Authorization
                    if (context.Request.Headers.ContainsKey("Authorization"))
                    {
                        var authHeader = context.Request.Headers["Authorization"].ToString();
                        var token = authHeader.StartsWith("Bearer ") ? authHeader.Substring("Bearer ".Length).Trim() : authHeader;
                        Log.Information("JWT token received: {Token}", token);
                        if (token == null)
                        {
                            Log.Information("Token is null");
                        }
                    }


                    context.Request.Body.Position = 0; // Вернуть поток на начало

                    await next();

                    Log.Information("Response status code: {StatusCode}", context.Response.StatusCode);
                });


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
