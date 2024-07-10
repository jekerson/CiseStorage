namespace Application.DTOs.Employee
{
    public record UpdateEmployeeDto(
        int Id,
        string Name,
        string Surname,
        string Lastname,
        string Phone,
        string Email,
        string Sex,
        int Age,
        int UserId
    );
}