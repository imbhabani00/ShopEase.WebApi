using Ecommerce.Application.DTOs.Request;
using FluentValidation;

namespace Ecommerce.Application.Validators
{
    public class InitiatePaymentRequestValidator : AbstractValidator<InitiatePaymentRequest>
    {
        public InitiatePaymentRequestValidator()
        {
            RuleFor(x => x.OrderId)
                .GreaterThan(0).WithMessage("Valid OrderId is required.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0.");

            RuleFor(x => x.Currency)
                .NotEmpty()
                .Length(3).WithMessage("Currency must be a 3-letter code (e.g. INR, USD).");
        }
    }
}