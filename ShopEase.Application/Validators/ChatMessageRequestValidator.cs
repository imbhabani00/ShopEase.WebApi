using Ecommerce.Application.DTOs.Request;
using FluentValidation;

namespace Ecommerce.Application.Validators
{
    public class ChatMessageRequestValidator : AbstractValidator<ChatMessageRequest>
    {
        public ChatMessageRequestValidator()
        {
            RuleFor(x => x.Message)
               .NotEmpty().WithMessage("Message cannot be empty.")
               .MaximumLength(1000).WithMessage("Message cannot exceed 1000 characters.");

            RuleFor(x => x.SessionId)
                .NotEmpty().WithMessage("SessionId is required.");
        }
    }
}