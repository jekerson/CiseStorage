using Application.Abstraction.Messaging;
using Application.DTOs.Attributes;

namespace Application.Attributes.UnitCategories.Query
{
    public record GetAllUnitCategoriesQuery : IQuery<IEnumerable<UnitCategoryDto>>;

}