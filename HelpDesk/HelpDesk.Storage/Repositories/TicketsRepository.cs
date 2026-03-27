using HelpDesk.Core.Abstractions;
using HelpDesk.Core.Models;
using HelpDesk.Shared.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
            var ticket = await Context.Tickets
                .AsNoTracking()
                .Include(t => t.Comments)
                    .ThenInclude(c => c.User)
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (ticket != null)
            {
                ticket.Comments = ticket.Comments
                    .OrderBy(c => c.CreatedDate)
                    .ToList();
            }

            return ticket;
        }

        public async ValueTask<Ticket?> GetForUpdate(Guid id)
        {
            return await Context.Tickets
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async ValueTask<Ticket[]> GetAll(PaginationQuery pagination)
        {
            return await Context.Tickets
                .AsNoTracking()
                .Skip(pagination.Offset)
                .Take(pagination.Limit)
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .ToArrayAsync();
        }

        public async ValueTask<Ticket[]> GetManyWithStatus(FilterSelectOptions options)
        {
            return await Context.Tickets
                .AsNoTracking()
                .Where(x => (options.Status == null || x.Status == options.Status))
                .Skip(options.Offset)
                .Take(options.Limit)
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .ToArrayAsync();
                
        }

        public async ValueTask<Ticket[]> GetAssignedToAdmin(Guid adminId, FilterSelectOptions options)
        {
            return await
                Context.Tickets
                .AsNoTracking()
                .Where(u => u.AssignedToId == adminId)
                .Skip(options.Offset)
                .Take(options.Limit)
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .ToArrayAsync();
        }

        public async ValueTask<Ticket[]> GetCreatedByUser(Guid userId, FilterSelectOptions options)
        {
            return await
               Context.Tickets
               .AsNoTracking()
               .Where(u => u.CreatedById == userId &&
                    (options.Status == null || u.Status == options.Status))
               .Skip(options.Offset)
               .Take(options.Limit)
               .Include(t => t.CreatedBy)
               .Include(t => t.AssignedTo)
               .ToArrayAsync();
        }
    }
}
