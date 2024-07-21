using Application.Abstraction.Messaging;
using Application.DTOs.Attributes.Measurement;

namespace Application.Attributes.AttributeUnit.Query.GetAll
{
    public record GetAllAttributeUnitsQuery : IQuery<IEnumerable<AttributeUnitDto>>;

}
