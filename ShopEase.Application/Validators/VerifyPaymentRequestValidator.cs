using Ecommerce.Application.DTOs.Request;
using FluentValidation;

namespace Ecommerce.Application.Validators
{
    public class VerifyPaymentRequestValidator : AbstractValidator<VerifyPaymentRequest>
    {
        public VerifyPaymentRequestValidator()
        {
            RuleFor(x => x.GatewayOrderId).NotEmpty().WithMessage("Gateway OrderId is required.");
            RuleFor(x => x.GatewayPaymentId).NotEmpty().WithMessage("Gateway PaymentId is required.");
            RuleFor(x => x.GatewaySignature).NotEmpty().WithMessage("Signature is required.");
            RuleFor(x => x.OrderId).GreaterThan(0).WithMessage("Valid OrderId is required.");
        }
    }
}
