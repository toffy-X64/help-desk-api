using HelpDesk.Core.Abstractions;
using HelpDesk.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Storage.Repositories
{
    public class UsersRepository : BaseRepository<User>, IUsersRepository
    {
        public UsersRepository(DataContext dataContext) : base(dataContext)
        {
        }

        public async ValueTask<User[]> Get(SearchOptions options)
        {
            return await
                Context.Users
                .AsNoTracking()
                .Where(u => !u.IsDeleted && EF.Functions.Like(u.Name, $"%{options.SearchText}%"))
                .Skip(options.Offset)
                .Take(options.Limit)
                .ToArrayAsync();
        }

        public async ValueTask<User?> Get(Guid id)
        {
            return await Context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async ValueTask<User?> GetByEmail(string email)
        {
            return await Context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> IsEmailTaken(string email)
        {
            return await Context.Users.AnyAsync(u => u.Email == email);
        }
    }
}
