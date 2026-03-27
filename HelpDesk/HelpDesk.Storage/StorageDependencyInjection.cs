using HelpDesk.Core.Abstractions;
using HelpDesk.Storage.Logic;
using HelpDesk.Storage.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Storage
{
    public static class StorageDependencyInjection
    {
        public static IServiceCollection AddStorage(this IServiceCollection services, IConfiguration configuration)
        {
            var sqlServerConnectionString = configuration.GetConnectionString("SqlServer") ??
                throw new Exception("Connection string `SqlServer` not found.");

            services.AddDbContext<DataContext>(options =>
                options
                    .UseSqlServer(sqlServerConnectionString)
            );

            services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<DataContext>());
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<ITicketsRepository, TicketsRepository>();

            return services;
        }
    }
}
