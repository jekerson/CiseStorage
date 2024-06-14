namespace Domain.Entities;

public partial class Employee
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string Sex { get; set; } = null!;

    public int Age { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public string EmailAddress { get; set; } = null!;

    public int? AddressId { get; set; }

    public int? PositionId { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual Address? Address { get; set; }

    public virtual ICollection<ItemResponsibility> ItemResponsibilities { get; set; } = new List<ItemResponsibility>();

    public virtual Position? Position { get; set; }

    public virtual ICollection<UserInfo> UserInfos { get; set; } = new List<UserInfo>();
}
