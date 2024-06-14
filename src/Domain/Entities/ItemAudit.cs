namespace Domain.Entities;

public partial class ItemAudit
{
    public int Id { get; set; }

    public int? ItemId { get; set; }

    public string? Name { get; set; }

    public string? Number { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CategoryId { get; set; }

    public DateTime? ChangedAt { get; set; }

    public int? ChangedBy { get; set; }

    public string Action { get; set; } = null!;

    public virtual UserInfo? ChangedByNavigation { get; set; }
}
