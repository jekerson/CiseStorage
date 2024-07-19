using Application.DTOs.Addresses;

namespace Application.DTOs.Employee
{
    public record UpdateEmployeeAddressDto(int EmployeeId, AddAddressDto NewAddress);
}