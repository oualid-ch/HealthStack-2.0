using FluentValidation;
using HealthStack.Auth.Api.DTOs;

namespace HealthStack.Auth.Api.Validators;

public class UserRegisterValidator : AbstractValidator<UserRegisterDto>
{
    public UserRegisterValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(100);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Password must contain an uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain a lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain a digit.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain a special character.");
    
        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20)
            .When(x => x.PhoneNumber != null);

        // Validate nested addresses
        RuleForEach(x => x.Addresses)
            .SetValidator(new AddressCreateDtoValidator());
    }
}
