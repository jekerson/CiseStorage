using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class UserRole
{
    public int Id { get; set; }

    public int? UserInfoId { get; set; }

    public int? RoleId { get; set; }

    public virtual Role? Role { get; set; }

    public virtual UserInfo? UserInfo { get; set; }
}
