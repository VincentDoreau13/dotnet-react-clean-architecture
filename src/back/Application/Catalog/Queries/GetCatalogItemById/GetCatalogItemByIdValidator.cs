using FluentValidation;

namespace ShopApi.Application.Catalog.Queries.GetCatalogItemById;

public class GetCatalogItemByIdValidator : AbstractValidator<GetCatalogItemByIdQuery>
{
    public GetCatalogItemByIdValidator()
    {
        RuleFor(catalogItem => catalogItem.Id)
            .GreaterThan(0).WithMessage("Id must be greater than 0.");
    }
}
