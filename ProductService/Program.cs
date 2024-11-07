using ProductService.Api;

namespace ProductService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            IConfiguration configuration = builder.Configuration;

            builder.Services.AddSwagger();
            builder.Services.AddControllers();
            builder.Services.AddAuthentication(configuration);
            builder.Services.AddAuthorization();
            builder.Services.AddServices();
            builder.Services.AddValidation();
            builder.Services.AddEntityFramework(configuration);
            var app = builder.Build();

            app.MapControllers();
            app.UseSwagger(app);

            app.UseMiddlewares();
            app.UseAuthentication();
            app.UseAuthorization();



            app.Run();
        }
    }
}
