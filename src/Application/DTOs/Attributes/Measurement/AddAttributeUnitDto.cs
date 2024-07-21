namespace Application.DTOs.Attributes.Measurement
{
    public record AddAttributeUnitDto(
        string Name,
        string Symbol,
        int UnitCategoryId,
        int AttributeValueTypeId
    );
}