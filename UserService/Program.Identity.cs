using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserService.Models;
using UserService.Infrastructure.Database; 

namespace UserService.Api;

public static class ProgramIdentity

{
    public static IServiceCollection AddIdentity(this IServiceCollection services) 
    {

        services.AddIdentity<User, IdentityRole>()
                    .AddEntityFrameworkStores<DatabaseContext>()
                    .AddDefaultTokenProviders();

        return services;
    }
}
