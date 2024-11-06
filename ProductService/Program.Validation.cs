using FluentValidation;
using FluentValidation.AspNetCore;
using ProductService.Api.Controllers.Products.Requests;
using ProductService.Api.Controllers.Products.Validators;

namespace ProductService.Api
{
    public static class ProgramValidation
    {
       public static IServiceCollection AddValidation(this IServiceCollection services)
        {
            services.AddScoped<IValidator<CreateProductRequest>,CreateProductRequestValidator>();
            services.AddScoped<IValidator<UpdateProductRequest>, UpdateProductRequestValidator>();
            services.AddFluentValidationAutoValidation();
            return services;
        }
        
    }
}
