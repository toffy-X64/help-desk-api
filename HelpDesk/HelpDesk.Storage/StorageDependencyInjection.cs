using HelpDesk.Core.Abstractions;
using HelpDesk.Storage.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            services.AddDbContext<DataContext>(options => options.UseSqlServer(sqlServerConnectionString));

            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<ITicketsRepository, TicketsRepository>();

            return services;
        }
    }
}
