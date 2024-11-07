using Microsoft.EntityFrameworkCore;
using System.Drawing.Text;
using UserService.Infrastructure.Database;

namespace UserService.Api
{
    public static class ProgramEntityFramework
    {
        public static IServiceCollection AddEntityFramework(this IServiceCollection services, IConfiguration configuration)
        {
            // Настройка SQL Server
            services.AddDbContext<DatabaseContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));


            
            return services;
        }
    }
}
