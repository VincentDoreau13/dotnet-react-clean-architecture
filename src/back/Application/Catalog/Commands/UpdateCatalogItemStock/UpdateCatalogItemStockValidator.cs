using FluentValidation;

namespace ShopApi.Application.Catalog.Commands.UpdateCatalogItemStock;

public class UpdateCatalogItemStockValidator : AbstractValidator<UpdateCatalogItemStockCommand>
{
    public UpdateCatalogItemStockValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0).WithMessage("Id must be greater than 0.");

        RuleFor(command => command.AvailableStock)
            .GreaterThanOrEqualTo(0).WithMessage("AvailableStock must be 0 or more.")
            .LessThanOrEqualTo(1_000_000).WithMessage("AvailableStock must not exceed 1000000.");
    }
}
