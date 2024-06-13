using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Item
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Number { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public int? CategoryId { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ItemCategory? Category { get; set; }

    public virtual ICollection<ItemAttributeValue> ItemAttributeValues { get; set; } = new List<ItemAttributeValue>();

    public virtual ICollection<ItemResponsibility> ItemResponsibilities { get; set; } = new List<ItemResponsibility>();
}
