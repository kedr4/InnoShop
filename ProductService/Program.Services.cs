using ProductService.Domain.Products;
using ProductService.Infrastructure.Database.Repositories;

namespace ProductService
{
    public static class ProgramServices
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddMediator();
            services.AddScoped<IProductRepository, ProductRepository>();


            return services;
        }



    }
}
