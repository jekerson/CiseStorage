using Application.Abstraction.Messaging;
using Application.DTOs.Attributes;

namespace Application.Attributes.Attribute.Query.GetAll
{
    public record GetAllAttributesQuery : IQuery<IEnumerable<AttributeDto>>;

}
