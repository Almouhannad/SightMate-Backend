using IdentityService.Config;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Interfaces;
using IdentityService.Infrastructure.Users;
using IdentityService.Infrastructure.Users.DAOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace IdentityService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services
            .AddDbContextFromInfrastructure()
            .AddIdentityFromInfrastructure()
            .AddAuthorizationFromInfrastructure()
            .AddAuthenticationFromInfrastructure();

        services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(classes => classes.AssignableTo(typeof(IJWTProvider)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        services.RegisterUserContextImplementationFromInfrastructure();
        return services;
    }
    public static IServiceCollection AddDbContextFromInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(CONFIG.ConnectionString);
        });
        return services;
    }

    public static IServiceCollection AddIdentityFromInfrastructure(this IServiceCollection services)
    {
        services.AddIdentity<UserDAO, RoleDAO>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        return services;
    }

    public static IServiceCollection AddAuthenticationFromInfrastructure(this IServiceCollection services)
    {

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            //options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = CONFIG.JWTIssuer,
                ValidAudience = CONFIG.JWTAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(CONFIG.JWTSecretKey)),
                RoleClaimType = ClaimTypes.Role
            };
        });
        return services;
    }

    public static IServiceCollection AddAuthorizationFromInfrastructure(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
            {
                policy.RequireRole(Roles.ADMIN.Name);
            });
        });
        return services;
    }
    public static IServiceCollection RegisterUserContextImplementationFromInfrastructure(this IServiceCollection services)
    {
        services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(classes => classes.AssignableTo(typeof(IUserManager)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());
        return services;
    }
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        using ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Database.Migrate();
    }

}
