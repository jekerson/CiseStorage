namespace Application.DTOs.Attributes.Measurement
{
    public record AttributeUnitDto(
        string Name,
        string Symbol,
        string UnitCategory,
        string ValueType
    );
}