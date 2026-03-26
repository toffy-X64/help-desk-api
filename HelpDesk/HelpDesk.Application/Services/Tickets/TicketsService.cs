using HelpDesk.Application.Services.Tickets.DTO;
using HelpDesk.Application.Services.Tickets.Requests;

using HelpDesk.Core.Abstractions;
using HelpDesk.Core.Models;
using HelpDesk.Core.Models.Tickets;

using HelpDesk.Shared.Common;
using HelpDesk.Storage.Repositories;

namespace HelpDesk.Application.Services.Tickets
{
    public class TicketsService : BaseService
    {
        private readonly ITicketsRepository _ticketsRepository;
        private readonly IUsersRepository _usersRepository;

        public TicketsService(
            IUnitOfWork unitOfWork,
            ITicketsRepository ticketsRepository,
            IUsersRepository usersRepository
        ) 
            : base(unitOfWork)
        {
            _ticketsRepository = ticketsRepository;
            _usersRepository = usersRepository;
        }

        public async Task<Result<TicketDto>> CreateTicket(Guid clientId, CreateTicketRequest request)
        {
            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                Status = TicketStatuses.Created,
                CreatedById = clientId,
                CreatedDate = DateTime.UtcNow,
            };
            _ticketsRepository.Add(ticket);

            var systemComment = new Comment
            {
                Id = Guid.NewGuid(),
                TicketId = ticket.Id,
                Type = CommentTypes.System,
                UserId = null,
                Text = "Your ticket has been successfully created! Please wait, your support team will help you as soon, as possible!",
                CreatedDate = DateTime.UtcNow
            };
            _ticketsRepository.AddComment(systemComment);

            await UnitOfWork.SaveChangesAsync();

            var createdTicket = await _ticketsRepository.Get(ticket.Id);
            if (createdTicket == null)
            {
                return new Error("Failed to retrieve created ticket");
            }

            return MapToDto(createdTicket);
        }

        public async Task<Result<TicketDto>> AssignTicket(Guid supportId, Guid ticketId)
        {
            var ticket = await _ticketsRepository.Get(ticketId);
            if (ticket == null)
            {
                return new Error("Failed to retrieve ticket");
            }
            if (ticket.AssignedToId != null)
            {
                return new Error("This ticket is already assigned by Support");
            }

            var user = await _usersRepository.Get(supportId);
            if (user == null)
                return new Error("Support agent not found");

            if (!user.CanAssignTicket())
            {
                return new Error("Only support agents can assign tickets");
            }

            ticket.AssignedToId = supportId;
            ticket.Status = TicketStatuses.InProgress;
            _ticketsRepository.Update(ticket);

            var systemComment = new Comment
            {
                Id = Guid.NewGuid(),
                TicketId = ticket.Id,
                Type = CommentTypes.System,
                Text = $"Ticket has been assigned to {user.Name} and is now in progress."
            };
            _ticketsRepository.AddComment(systemComment);

            await UnitOfWork.SaveChangesAsync();

            var currentTicket = await _ticketsRepository.Get(ticketId);
            if (currentTicket == null)
            {
                return new Error("Failed to retrieve ticket");
            }

            return MapToDto(currentTicket);
        }

        private TicketDto MapToDto(Ticket ticket) => new TicketDto
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Description = ticket.Description,
            Status = ticket.Status,
            AssignedToId = ticket.AssignedToId,
            CreatedById = ticket.CreatedById,
            CreatedDate = ticket.CreatedDate,
            Comments = ticket.Comments?.Select(MapToCommentDto).ToArray() ?? []
        };

        private CommentDto MapToCommentDto(Comment comment) => new CommentDto
        {
            Id = comment.Id,
            UserId = comment.UserId,
            Type = comment.Type,
            Text = comment.Text,
            CreatedDate = comment.CreatedDate
        };
    }
}
