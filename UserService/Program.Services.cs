﻿using UserService.Application.Users.Commands.Create.UserService.Infrastructure.Email;
using UserService.Domain.Users;
using UserService.Infrastructure.Database.Repositories;
using UserService.Infrastructure.Email;

namespace UserService.Api
{
    public static class ProgramServices
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddMediator();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEmailService, EmailService>();////////////////


            return services;
        }



    }
}
