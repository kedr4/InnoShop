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
            // ��������� Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                Log.Information("Starting web host");
                var builder = WebApplication.CreateBuilder(args);
                IConfiguration config = builder.Configuration;

                // �������� �������� JWT �� appsettings.json
                  var jwtSettings = builder.Configuration.GetSection("JwtSettings");
                 var key = jwtSettings["Secret"];
                 var issuer = jwtSettings["Issuer"];
                 var audience = jwtSettings["Audience"];

                // ��������� in-memory ���� ������
                builder.Services.AddDbContext<UserContext>(opt =>
                    opt.UseInMemoryDatabase("UserDb"));

                // ��������� Identity
                builder.Services.AddIdentity<User, IdentityRole>()
                    .AddEntityFrameworkStores<UserContext>()
                    .AddDefaultTokenProviders();

                // ��������� �������������� JWT
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

                    // ����������� ������� ��������������
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
                // ���������� �������� � ���������
                builder.Services.AddControllers();


                // ��������� ����������� Serilog
                builder.Host.UseSerilog();


               

                

                // ��������� Swagger
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "UserService", Version = "v1" });

                    // ���������� ����� ������������ ��� JWT
                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "������� 'Bearer' [������] � ��� ����� � ���� ����. \r\n\r\n������: \"Bearer 12345abcdef\"",
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

                // ��������� HTTP request pipeline
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();

                // Middleware ��� ����������� �������� � �������
                app.Use(async (context, next) =>
                {
                    Log.Information("Received request: {Method} {Path}", context.Request.Method, context.Request.Path);

                    // �������� ���� �������
                    context.Request.EnableBuffering(); // ��������� ������ ���� ������� ��������� ���
                    var bodyAsText = await new StreamReader(context.Request.Body).ReadToEndAsync();
                    Log.Information("Request body: {Body}", bodyAsText);

                    // �������� ����� �� ��������� Authorization
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


                    context.Request.Body.Position = 0; // ������� ����� �� ������

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
