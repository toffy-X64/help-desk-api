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
    public class TicketsRepository : BaseRepository<Ticket>, ITicketsRepository
    {
        public TicketsRepository(DataContext dataContext) : base(dataContext)
        { 
        }

        public void AddComment(Comment comment)
        {
            Context.Comments.Add(comment);
        }

        public async ValueTask<Ticket?> Get(Guid id)
        {
            return await Context.Tickets.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async ValueTask<Ticket[]> GetAssignedToAdmin(Guid adminId, FilterSelectOptions options)
        {
            return await
                Context.Tickets
                .Where(u => u.AssignedToId == adminId)
                .Skip(options.Offset)
                .Take(options.Limit)
                .ToArrayAsync();
        }

        public async ValueTask<Ticket[]> GetCreatedByUser(Guid userId, FilterSelectOptions options)
        {
            return await
               Context.Tickets
               .Where(u => u.CreatedById == userId)
               .Skip(options.Offset)
               .Take(options.Limit)
               .ToArrayAsync();
        }
    }
}
