using Application.DTOs.Attributes;

namespace Application.Services.Attributes.Datatype
{
    public interface IAttributeValueTypeService
    {
        IEnumerable<AttributeValueTypeDto> GetAllAttributeValueTypes();
    }
}