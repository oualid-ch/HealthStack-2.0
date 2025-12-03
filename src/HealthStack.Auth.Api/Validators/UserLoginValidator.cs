using FluentValidation;
using HealthStack.Auth.Api.DTOs;

namespace HealthStack.Auth.Api.Validators;

public class UserLoginValidator : AbstractValidator<UserLoginDto>
{
    public UserLoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(100);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);
    }
}
