using Application.DTOs.Addresses;

namespace Application.DTOs.Employee
{
    public record AddEmployeeDto(
        string Name,
        string Surname,
        string Lastname,
        string Phone,
        string Email,
        string Sex,
        int Age,
        int PositionId,
        int UserId,
        AddAddressDto AddAddressDto
    );
}