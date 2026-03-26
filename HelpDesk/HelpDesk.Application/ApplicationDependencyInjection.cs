using FluentValidation;
using HelpDesk.Application.Logic;
using HelpDesk.Application.Services.Auth;
using HelpDesk.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Application
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<JwtService>();
            services.AddScoped<IValidator<RegisterRequest>, RegisterValidator>();
            services.AddScoped<IValidator<LoginRequest>, LoginValidator>();

            services.AddScoped<AuthService>();

            services.AddStorage(configuration);

            return services;
        }
    }
}
