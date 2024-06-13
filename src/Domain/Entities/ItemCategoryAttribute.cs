using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class ItemCategoryAttribute
{
    public int Id { get; set; }

    public int? ItemCategoryId { get; set; }

    public int? AttributeId { get; set; }

    public virtual Attribute? Attribute { get; set; }

    public virtual ItemCategory? ItemCategory { get; set; }
}
