using FluentValidation;

namespace ShopApi.Application.Orders.Queries.GetOrderById;

public class GetOrderByIdValidator : AbstractValidator<GetOrderByIdQuery>
{
    public GetOrderByIdValidator()
    {
        RuleFor(query => query.Id)
            .GreaterThan(0).WithMessage("Id must be greater than 0.");
    }
}
