using FluentValidation;
using HelpDesk.Application.Services.Auth;
using HelpDesk.Application.Services.Tickets;
using HelpDesk.Application.Services.Tickets.Requests;
using HelpDesk.Application.Services.Tickets.Validator;
using HelpDesk.Application.Services.Users;
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
            services.AddStorage(configuration);

            services.AddScoped<IValidator<RegisterRequest>, RegisterValidator>();
            services.AddScoped<IValidator<LoginRequest>, LoginValidator>();
            services.AddScoped<IValidator<CreateTicketRequest>, CreateTicketValidator>();
            services.AddScoped<IValidator<CreateCommentRequest>, CreateCommentValidator>();

            services.AddScoped<AuthService>();
            services.AddScoped<UsersService>();
            services.AddScoped<TicketsService>();
            return services;
        }
    }
}
