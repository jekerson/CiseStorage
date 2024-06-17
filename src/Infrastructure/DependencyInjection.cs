using Application.Abstraction.Cache;
using Application.Abstraction.Messaging;
using Domain.Repositories.Addresses;
using Domain.Repositories.Item;
using Domain.Repositories.Role_permission;
using Domain.Repositories.Staff;
using Infrastructure.Authentication;
using Infrastructure.Cache;
using Infrastructure.Data;
using Infrastructure.Repositories.Addresses;
using Infrastructure.Repositories.Items;
using Infrastructure.Repositories.Role_permission;
using Infrastructure.Repositories.Staff;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<SiceDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("Database")));

            services.AddMemoryCache();
            services.AddScoped<ICacheProvider, InMemoryCacheProvider>();

            services.Configure<JwtOptions>(configuration.GetSection("Jwt"));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"])),
                        ValidateIssuer = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = configuration["Jwt:Audience"]
                    };
                });


            services.AddSingleton<IJwtProvider, JwtProvider>();

            services.AddAuthorization();
            services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();


            //repositories
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IRegionRepository, RegionRepository>();
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IItemRepository, ItemRepository>();


            return services;
        }
    }
}