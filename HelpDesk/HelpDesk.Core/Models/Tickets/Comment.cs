using HelpDesk.Core.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Core.Models
{
    public class Comment
    {
        public Guid Id { get; set; }

        public Guid TicketId { get; set; }
        public Ticket Ticket { get; set; }

        public Guid? UserId { get; set; }
        public User? User { get; set; }

        public string Type { get; set; }
        public string Text { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
