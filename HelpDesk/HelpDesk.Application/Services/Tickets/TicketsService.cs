using FluentValidation;
using FluentValidation.Results;

using HelpDesk.Application.Services.Auth;
using HelpDesk.Application.Services.Tickets.DTO;
using HelpDesk.Application.Services.Tickets.Requests;

using HelpDesk.Core.Abstractions;
using HelpDesk.Core.Models;
using HelpDesk.Core.Models.Tickets;

using HelpDesk.Shared.Common;

namespace HelpDesk.Application.Services.Tickets
{
    public class TicketsService : BaseService
    {
        private readonly ITicketsRepository _ticketsRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IValidator<CreateTicketRequest> _createTicketValidator;
        private readonly IValidator<CreateCommentRequest> _createCommentValidator;

        public TicketsService(
            IUnitOfWork unitOfWork,
            ITicketsRepository ticketsRepository,
            IUsersRepository usersRepository,
            IValidator<CreateTicketRequest> createTicketValidator,
            IValidator<CreateCommentRequest> createCommentValidator
        ) 
            : base(unitOfWork)
        {
            _ticketsRepository = ticketsRepository;
            _usersRepository = usersRepository;
            _createTicketValidator = createTicketValidator;
            _createCommentValidator = createCommentValidator;
        }

        public async Task<Result<TicketDto>> CreateTicket(Guid clientId, CreateTicketRequest request)
        {
            ValidationResult validationResult = await _createTicketValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var firstError = validationResult.Errors.First();
                return new Error(firstError.ErrorMessage, firstError.PropertyName);
            }

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

            ticket.Comments ??= new List<Comment>();
            ticket.Comments.Add(systemComment);

            return MapToDto(ticket);
        }

        public async Task<Result<TicketDto>> AssignTicket(Guid supportId, Guid ticketId)
        {
            await using var transaction = await UnitOfWork.BeginTransactionAsync();

            try
            {
                // Ticket
                var ticket = await _ticketsRepository.Get(ticketId);
                if (ticket == null)
                {
                    return new Error("Failed to retrieve ticket");
                }

                // Support agent
                var user = await _usersRepository.Get(supportId);
                if (user == null)
                    return new Error("Support agent not found");
                
                // Assign (Update)
                Result assignResult = ticket.AssignTo(user);
                if (!assignResult.IsSuccess)
                    return assignResult.Error ?? new Error("Unable to assign ticket");

                _ticketsRepository.Update(ticket);

                // Comment (system)
                var systemComment = new Comment
                {
                    Id = Guid.NewGuid(),
                    TicketId = ticket.Id,
                    Type = CommentTypes.System,
                    Text = $"Ticket has been assigned to {user.Name} and is now in progress.",
                    CreatedDate = DateTime.UtcNow
                };
                _ticketsRepository.AddComment(systemComment);

                await UnitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                ticket.Comments ??= new List<Comment>();
                ticket.Comments.Add(systemComment);

                return MapToDto(ticket);
            }
            catch (Exception ex) 
            {
                throw;
            }
        }

        public async Task<Result<CommentDto>> CreateComment(Guid senderId, Guid ticketId, CreateCommentRequest request)
        {
            ValidationResult validationResult = await _createCommentValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var firstError = validationResult.Errors.First();
                return new Error(firstError.ErrorMessage, firstError.PropertyName);
            }

            var user = await _usersRepository.Get(senderId);
            if (user == null)
                return new Error("Sender not found");

            var ticket = await _ticketsRepository.Get(ticketId);
            if (ticket == null)
                return new Error("Ticket not found");

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                TicketId = ticketId,
                Text = request.Text,
            };
            Result sendingResult = ticket.SendComment(user, comment);
            if (!sendingResult.IsSuccess)
                return sendingResult.Error ?? new Error("Unable to send comment");

            _ticketsRepository.AddComment(comment);
            await UnitOfWork.SaveChangesAsync();

            comment.User = user;

            return MapToCommentDto(comment);
        }

        public async Task<Result<TicketDto>> ResolveTicket(Guid supportId, Guid ticketId)
        {
            var support = await _usersRepository.Get(supportId);
            if (support == null)
                return new Error("Support not found");

            var ticket = await _ticketsRepository.GetForUpdate(ticketId);
            if (ticket == null)
                return new Error("Ticket not found");

            Result resolveResult = ticket.Resolve(support);
            if (!resolveResult.IsSuccess)
                return resolveResult.Error ?? new Error("Unable to resolve ticket");

            _ticketsRepository.Update(ticket);
            await UnitOfWork.SaveChangesAsync();

            var resolvedTicket = await _ticketsRepository.Get(ticketId);
            return MapToDto(resolvedTicket);
        }

        public async Task<Result<IEnumerable<TicketDto>>> GetAllTickets(PaginationQuery pagination)
        {
            var tickets = (await _ticketsRepository.GetAll(pagination)).Select(MapToDto).ToArray();
            return tickets;
        }

        public async Task<Result<IEnumerable<TicketDto>>> GetAllTicketsAssignedToSupport(Guid supportId, PaginationQuery pagination)
        {
            var support = await _usersRepository.Get(supportId);
            if (support == null)
                return new Error("Support not found");

            var tickets = (await _ticketsRepository.GetAssignedToAdmin(supportId, new FilterSelectOptions {
                Limit = pagination.Limit,
                Offset = pagination.Offset
            })).Select(MapToDto).ToArray();

            return tickets;
        }

        public async Task<Result<IEnumerable<TicketDto>>> GetAllPendingTickets(PaginationQuery pagination)
        {
            var tickets = (await _ticketsRepository.GetManyWithStatus(new FilterSelectOptions
            {
                Limit = pagination.Limit,
                Offset = pagination.Offset,
                Status = TicketStatuses.Created
            })).Select(MapToDto).ToArray();

            return tickets;
        }

        public async Task<Result<IEnumerable<TicketDto>>> GetAllCreatedByUser(Guid userId, PaginationQuery pagination)
        {
            var tickets = (await _ticketsRepository.GetCreatedByUser(userId, new FilterSelectOptions
            {
                Limit = pagination.Limit,
                Offset = pagination.Offset
            })).Select(MapToDto).ToArray();

            return tickets;
        }

        public async Task<Result<TicketDto>> GetTicket(Guid userId, Guid ticketId)
        {
            var user = await _usersRepository.Get(userId);
            if (user == null)
                return new Error("User not found");

            var ticket = await _ticketsRepository.Get(ticketId);
            if (ticket == null)
                return new Error("Ticket not found");

            if (ticket.CreatedById != userId && ticket.AssignedToId != userId && !user.CanManipulateTickets())
            {
                return new Error("Access denied");
            }

            return MapToDto(ticket);
        }


        // Mappers
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
            User = comment.User != null ? MapToUserDto(comment.User) : null,
            Type = comment.Type,
            Text = comment.Text,
            CreatedDate = comment.CreatedDate
        };

        private UserDto MapToUserDto(User user) => new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            Role = user.Role
        };
    }
}
