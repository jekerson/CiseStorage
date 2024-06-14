namespace Domain.Entities;

public partial class AttributeValueType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<AttributeUnit> AttributeUnits { get; set; } = new List<AttributeUnit>();
}
