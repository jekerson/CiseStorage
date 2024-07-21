using Application.Abstraction.Messaging;
using Application.DTOs.Attributes;

namespace Application.Attributes.AttributeCategory.Query
{
    public record GetAllAttributeCategoriesQuery : IQuery<IEnumerable<AttributeCategoryDto>>;

}
