namespace Domain.Entities;

public partial class ItemResponsibility
{
    public int Id { get; set; }

    public int? ItemId { get; set; }

    public int? EmployeeId { get; set; }

    public DateTime? AssignedAt { get; set; }

    public DateTime? UnassignedAt { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual Item? Item { get; set; }
}
