namespace Application.DTOs.Employee
{
    public record EmployeeWithoutDetailsDto(
        int Id,
        string Name,
        string Surname,
        string Lastname,
        string Phone,
        string PositionName);
}
