namespace Domain.Entities;

public partial class AttributeUnit
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Symbol { get; set; } = null!;

    public int? UnitCategoryId { get; set; }

    public int? AttributeValueTypeId { get; set; }

    public virtual AttributeValueType? AttributeValueType { get; set; }

    public virtual ICollection<Attribute> Attributes { get; set; } = new List<Attribute>();

    public virtual UnitCategory? UnitCategory { get; set; }
}
