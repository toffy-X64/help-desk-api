using HelpDesk.Core.Models.Tickets;
using HelpDesk.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Core.Models
{
    public class Ticket
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }

        public Guid? AssignedToId { get; set; }
        public User? AssignedTo { get; set; }
        public Guid CreatedById { get; set; }
        public User CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? AssignedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public ICollection<Comment> Comments { get; set; }

        public Result AssignTo(User supportAgent)
        {
            if (supportAgent == null)
                return Result.Failure(new Error("Support agent not found"));

            if (!supportAgent.CanManipulateTickets())
                return Result.Failure(new Error("Only support agents can assign tickets"));

            if (AssignedToId != null)
                return Result.Failure(new Error("This ticket is already assigned to a Support agent"));

            AssignedToId = supportAgent.Id;
            Status = TicketStatuses.InProgress;
            AssignedAt = DateTime.UtcNow;

            return Result.Success();
        }

        public Result Resolve(User supportAgent)
        {
            if (supportAgent == null)
                return Result.Failure(new Error("Support agent not found"));

            if (!supportAgent.CanManipulateTickets())
                return Result.Failure(new Error("Only support agents can resolve tickets"));

            if (AssignedToId != supportAgent.Id)
                return new Error("You don't have permission to resolve this ticket");

            Status = TicketStatuses.Resolved;
            ResolvedAt = DateTime.UtcNow;

            return Result.Success();
        }

        public Result SendComment(User sender, Comment comment)
        {
            if (Status == TicketStatuses.Resolved)
                return Result.Failure(new Error("Ticket is already resolved"));
            if (AssignedToId != sender.Id && CreatedById != sender.Id)
                return Result.Failure(new Error("You don't have permission to send message in this ticket!"));

            // Comment role (type)
            string commentType;
            if (AssignedToId == sender.Id)
                commentType = CommentTypes.Support;
            else
                commentType = CommentTypes.User;

            comment.Type = commentType;
            comment.UserId = sender.Id;
            comment.CreatedDate = DateTime.UtcNow;

            Comments ??= new List<Comment>();
            Comments.Add(comment);

            return Result.Success();
        }
    }
}
