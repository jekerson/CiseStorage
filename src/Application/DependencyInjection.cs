using Application.Abstraction.Behavior;
using Application.Services.Addresses;
using Application.Services.Attributes.Datatype;
using Application.Services.Audit.Staff;
using Application.Services.Password;
using Application.Users.Auth.Registration;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;

            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IEmployeeAuditService, EmployeeAuditService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IAttributeValueTypeService, AttributeValueTypeService>();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

            services.AddValidatorsFromAssembly(assembly);

            services.AddValidatorsFromAssembly(typeof(UserRegistrationCommandValidator).Assembly);

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

            return services;
        }
    }
}
