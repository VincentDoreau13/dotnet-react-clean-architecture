using FluentValidation;

namespace ShopApi.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(command => command.Items)
            .NotEmpty().WithMessage("Order must contain at least one item.");

        RuleForEach(command => command.Items).ChildRules(orderItem =>
        {
            orderItem.RuleFor(item => item.CatalogItemId)
                .GreaterThan(0).WithMessage("CatalogItemId must be greater than 0.");

            orderItem.RuleFor(item => item.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
        });
    }
}
