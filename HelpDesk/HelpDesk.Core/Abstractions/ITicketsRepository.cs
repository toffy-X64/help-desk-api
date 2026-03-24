using HelpDesk.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Core.Abstractions
{
    public interface ITicketsRepository
    {
        void Add(Ticket ticket);
        void AddComment(Comment comment);
        void Update(Ticket ticket);
        ValueTask<Ticket?> Get(Guid id);
        ValueTask<Ticket[]> GetAssignedToAdmin(Guid adminId, FilterSelectOptions options);
        ValueTask<Ticket[]> GetCreatedByUser(Guid userId, FilterSelectOptions options);
    }

    public class FilterSelectOptions
    {
        public int Limit { get; set; } = 10;
        public int Offset { get; set; } = 0;
        public string? Status { get; set; }
    }
}
