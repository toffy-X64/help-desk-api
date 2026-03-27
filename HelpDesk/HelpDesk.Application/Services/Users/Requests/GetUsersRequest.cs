using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Application.Services.Users.Requests
{
    public class GetUsersRequest
    {
        public int Limit { get; set; } = 10;
        public int Offset { get; set; } = 0;
    }
}
