using FluentValidation;
using HealthStack.Catalog.Api.DTOs;

namespace HealthStack.Catalog.Api.Validators;
public class CategoryCreateValidator : AbstractValidator<CategoryCreateDto>
{
    public CategoryCreateValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500);
    }
}
