using Ecommerce.Application.DTOs.Request;
using FluentValidation;

namespace Ecommerce.Application.Validators
{
    public class RefundRequestValidator : AbstractValidator<RefundRequest>
    {
        public RefundRequestValidator()
        {
            RuleFor(x => x.GatewayPaymentId).NotEmpty().WithMessage("PaymentId is required.");
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Refund amount must be greater than 0.");
            RuleFor(x => x.OrderId).GreaterThan(0).WithMessage("Valid OrderId is required.");
        }
    }
}
