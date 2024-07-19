using Application.DTOs.Attributes;
using Domain.Enum;
using Domain.Extensions;

namespace Application.Services.Attributes.Datatype
{
    public class AttributeValueTypeService : IAttributeValueTypeService
    {
        public IEnumerable<AttributeValueTypeDto> GetAllAttributeValueTypes()
        {
            return Enum.GetValues(typeof(AttributeValueDataType))
                .Cast<AttributeValueDataType>()
                .Select(value => new AttributeValueTypeDto(
                    (int)value,
                    value.ToString(),
                    value.GetDatatypeDisplayName()
                ));
        }
    }
}