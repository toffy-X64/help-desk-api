using HelpDesk.Application.Services.Auth;
using HelpDesk.Application.Services.Tickets.DTO;
using HelpDesk.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Application.Services.Tickets.Requests
{
    public class CreateCommentRequest
    {
        public string Text { get; set; }
    }
}
