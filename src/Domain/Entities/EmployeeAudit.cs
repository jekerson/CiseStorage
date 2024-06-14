namespace Domain.Entities;

public partial class EmployeeAudit
{
    public int Id { get; set; }

    public int? EmployeeId { get; set; }

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public string? Lastname { get; set; }

    public string? Sex { get; set; }

    public int? Age { get; set; }

    public string? PhoneNumber { get; set; }

    public string? EmailAddress { get; set; }

    public int? AddressId { get; set; }

    public int? PositionId { get; set; }

    public DateTime? ChangedAt { get; set; }

    public int? ChangedBy { get; set; }

    public string Action { get; set; } = null!;

    public virtual UserInfo? ChangedByNavigation { get; set; }
}
