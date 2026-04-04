using FluentValidation;

namespace ShopApi.Application.Catalog.Commands.CreateCatalogItem;

public class CreateCatalogItemValidator : AbstractValidator<CreateCatalogItemCommand>
{
    public CreateCatalogItemValidator()
    {
        RuleFor(catalogItem => catalogItem.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(catalogItem => catalogItem.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

        RuleFor(catalogItem => catalogItem.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.");

        RuleFor(catalogItem => catalogItem.AvailableStock)
            .GreaterThanOrEqualTo(0).WithMessage("AvailableStock must be 0 or more.");
    }
}
