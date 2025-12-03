using FluentValidation;
using HealthStack.Auth.Api.DTOs;

namespace HealthStack.Auth.Api.Validators
{
    public class AddressCreateDtoValidator : AbstractValidator<AddressCreateDto>
    {
        public AddressCreateDtoValidator()
        {
            RuleFor(x => x.Street)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.City)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.State)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.PostalCode)
                .NotEmpty()
                .MaximumLength(20);

            RuleFor(x => x.Country)
                .NotEmpty()
                .MaximumLength(50);
        }
    }
}
