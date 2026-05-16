using FluentValidation;
using Zent.Common;

namespace Zent.API.Endpoints.Auth.Register;

public sealed class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .Matches(ValidationPatterns.Email)
            .WithMessage("Invalid email address.");
        
        RuleFor(x => x.Password)
            .Matches(ValidationPatterns.Password)
            .WithMessage("Password must be at least 8 characters long and contain at least one letter.");
        
        RuleFor(x => x.FirstName).Length(2, 20);
        RuleFor(x => x.LastName).Length(2, 20);
    }
}