using Ecommerce.Application.DTOs.Request;
using FluentValidation;

namespace Ecommerce.Application.Validators
{
    public class GpsLocationRequestValidator : AbstractValidator<GpsLocationRequest>
    {
        public GpsLocationRequestValidator()
        {
            RuleFor(x => x.Latitude)
               .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.");

            RuleFor(x => x.OrderId)
                .GreaterThan(0).WithMessage("Valid OrderId is required.");
        }
    }
}