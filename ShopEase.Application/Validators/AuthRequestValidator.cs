using Ecommerce.Domain.Models;
using FluentValidation;

namespace ShopEase.Application.Validators
{
    public class AuthRequestValidator : AbstractValidator<AuthModel>
    {
        public AuthRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.");
            RuleFor(x => x.PasswordHash).NotEmpty().WithMessage("Password is required.");
        }
    }
}