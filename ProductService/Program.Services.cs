using Microsoft.OpenApi.Models;
using ProductService.Domain.Products;
using ProductService.Infrastructure.Database.Repositories;
using System.Collections;
using System.Runtime.CompilerServices;

namespace ProductService
{
    public static class ProgramServices
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddMediator();
            services.AddScoped<IProductRepository,ProductRepository>();


            return services;
        }

       

    }
}
