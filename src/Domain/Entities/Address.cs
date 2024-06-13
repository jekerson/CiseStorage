using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Address
{
    public int Id { get; set; }

    public string City { get; set; } = null!;

    public string Street { get; set; } = null!;

    public string House { get; set; } = null!;

    public string? Building { get; set; }

    public string Apartment { get; set; } = null!;

    public int? RegionId { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual Region? Region { get; set; }
}
