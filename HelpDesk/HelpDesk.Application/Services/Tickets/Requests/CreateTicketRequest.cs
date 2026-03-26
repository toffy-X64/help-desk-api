using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Application.Services.Tickets.Requests
{
    public class CreateTicketRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
