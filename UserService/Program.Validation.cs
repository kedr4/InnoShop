using FluentValidation;
using FluentValidation.AspNetCore;
using ProductService.Api.Controllers.Products.Requests;
using UserService.Api.Controllers.Users.Requests;
//using UserService.Api.Controllers.Users.Validators;

namespace UserService.Api
{
    public static class ProgramValidation
    {
       public static IServiceCollection AddValidation(this IServiceCollection services)
        {
            //services.AddScoped<IValidator<CreateUserRequest>,CreateUserRequestValidators>();
            //services.AddScoped<IValidator<UpdateUserRequest>, UpdateProductRequestValidator>();
            //services.AddFluentValidationAutoValidation();
            return services;
        }
        
    }
}
