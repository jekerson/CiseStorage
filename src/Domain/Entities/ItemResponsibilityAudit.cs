using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class ItemResponsibilityAudit
{
    public int Id { get; set; }

    public int? ItemResponsibilityId { get; set; }

    public int? ItemId { get; set; }

    public int? EmployeeId { get; set; }

    public DateTime? AssignedAt { get; set; }

    public DateTime? UnassignedAt { get; set; }

    public DateTime? ChangedAt { get; set; }

    public int? ChangedBy { get; set; }

    public string Action { get; set; } = null!;

    public virtual UserInfo? ChangedByNavigation { get; set; }
}
