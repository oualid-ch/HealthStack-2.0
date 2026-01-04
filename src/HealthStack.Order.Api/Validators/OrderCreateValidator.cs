using FluentValidation;
using HealthStack.Order.Api.DTOs;

namespace HealthStack.Order.Api.Validators
{
    public class OrderCreateDtoValidator : AbstractValidator<OrderCreateDto>
    {
        public OrderCreateDtoValidator()
        {
            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Order must have at least one item.");

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.ProductId)
                    .NotEmpty().WithMessage("ProductId is required.");

                item.RuleFor(i => i.Quantity)
                    .GreaterThan(0).WithMessage("Quantity must be at least 1.");
            });

            // Optional: status is usually set server-side, so you can ignore it
            // RuleFor(x => x.Status).NotEmpty().WithMessage("Status is required.");
        }
    }
}
