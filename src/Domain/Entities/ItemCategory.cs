namespace Domain.Entities;

public partial class ItemCategory
{
    public int Id { get; set; }

    public int? ParentCategoryId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<ItemCategory> InverseParentCategory { get; set; } = new List<ItemCategory>();

    public virtual ICollection<ItemCategoryAttribute> ItemCategoryAttributes { get; set; } = new List<ItemCategoryAttribute>();

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    public virtual ItemCategory? ParentCategory { get; set; }
}
