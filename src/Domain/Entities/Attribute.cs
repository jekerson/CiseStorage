namespace Domain.Entities;

public partial class Attribute
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsRequired { get; set; }

    public int? AttributeCategoryId { get; set; }

    public int? AttributeUnitId { get; set; }

    public virtual AttributeCategory? AttributeCategory { get; set; }

    public virtual AttributeUnit? AttributeUnit { get; set; }

    public virtual ICollection<ItemAttributeValue> ItemAttributeValues { get; set; } = new List<ItemAttributeValue>();

    public virtual ICollection<ItemCategoryAttribute> ItemCategoryAttributes { get; set; } = new List<ItemCategoryAttribute>();
}
