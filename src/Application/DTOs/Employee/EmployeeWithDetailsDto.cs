using Application.DTOs.Items.ItemResponsibility;

namespace Application.DTOs.Employee
{
    public record EmployeeWithDetailsDto(
        string Id,
        string Name,
        string Surname,
        string Lastname,
        string Phone,
        string Email,
        string Sex,
        string Age,
        string Position,
        int AddressId,
        string FullAddress,
        IEnumerable<ItemResponsibilityWithoutDetails> ItemsResponsibility
    );
}
