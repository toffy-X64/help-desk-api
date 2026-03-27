using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Core.Models.Tickets
{
    public static class TicketStatuses
    {
        public static string Created => "PENDING";
        public static string InProgress => "IN_PROGRESS";
        public static string Resolved => "RESOLVED";
    }
}
