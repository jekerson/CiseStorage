namespace Application.DTOs.Items.ItemResponsibility
{
    public record ItemResponsibilityWithoutDetails(
        string ItemName,
        string ItemNumber,
        string AssignedAt
    );
}
