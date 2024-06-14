namespace Domain.Entities;

public partial class ItemAttributeValue
{
    public int Id { get; set; }

    public int? ItemId { get; set; }

    public int? AttributeId { get; set; }

    public string? Value { get; set; }

    public virtual Attribute? Attribute { get; set; }

    public virtual Item? Item { get; set; }
}
