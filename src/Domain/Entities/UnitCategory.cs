using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class UnitCategory
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<AttributeUnit> AttributeUnits { get; set; } = new List<AttributeUnit>();
}
