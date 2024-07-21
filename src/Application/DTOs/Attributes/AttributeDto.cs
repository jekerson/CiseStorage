using Application.DTOs.Attributes.Measurement;

namespace Application.DTOs.Attributes
{
    public record AttributeDto(
        int Id,
        string Name,
        bool IsRequired,
        string AttributeCategory,
        AttributeUnitDto AttributeUnitDto
        );
}
