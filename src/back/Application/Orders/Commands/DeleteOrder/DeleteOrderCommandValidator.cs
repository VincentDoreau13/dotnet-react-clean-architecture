using FluentValidation;

namespace ShopApi.Application.Orders.Commands.DeleteOrder;

public class DeleteOrderCommandValidator : AbstractValidator<DeleteOrderCommand>
{
    public DeleteOrderCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0).WithMessage("Order ID must be greater than 0.");
    }
}
