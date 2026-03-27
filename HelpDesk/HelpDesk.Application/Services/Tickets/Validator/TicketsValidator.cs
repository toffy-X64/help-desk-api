using FluentValidation;
using HelpDesk.Application.Services.Tickets.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Application.Services.Tickets.Validator
{
    public class CreateTicketValidator : AbstractValidator<CreateTicketRequest>
    {
        public CreateTicketValidator()
        {
            RuleFor(x => x.Title).MaximumLength(128).NotNull().NotEmpty();
            RuleFor(x => x.Description).MinimumLength(10).NotNull().NotEmpty();
        }
    }

    public class CreateCommentValidator : AbstractValidator<CreateCommentRequest> 
    { 
        public CreateCommentValidator()
        {
            RuleFor(x => x.Text).MinimumLength(10).NotNull().NotEmpty();
        }
    }
}
