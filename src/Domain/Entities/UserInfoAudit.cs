using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class UserInfoAudit
{
    public int Id { get; set; }

    public int? UserInfoId { get; set; }

    public int? EmployeeId { get; set; }

    public string? Username { get; set; }

    public string? Salt { get; set; }

    public string? HashedPassword { get; set; }

    public DateTime? ChangedAt { get; set; }

    public int? ChangedBy { get; set; }

    public string Action { get; set; } = null!;

    public virtual UserInfo? ChangedByNavigation { get; set; }
}
