using HelpDesk.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Application.Services.Tickets.DTO
{
    public class CommentDto
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
