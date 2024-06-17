namespace Domain.Entities;

public partial class UserInfo
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public string Username { get; set; } = null!;

    public string Salt { get; set; } = null!;

    public string HashedPassword { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual ICollection<EmployeeAudit> EmployeeAudits { get; set; } = new List<EmployeeAudit>();

    public virtual ICollection<ItemAudit> ItemAudits { get; set; } = new List<ItemAudit>();

    public virtual ICollection<ItemResponsibilityAudit> ItemResponsibilityAudits { get; set; } = new List<ItemResponsibilityAudit>();

    public virtual ICollection<UserInfoAudit> UserInfoAudits { get; set; } = new List<UserInfoAudit>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

}
