using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class AttributeCategory
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Attribute> Attributes { get; set; } = new List<Attribute>();
}
