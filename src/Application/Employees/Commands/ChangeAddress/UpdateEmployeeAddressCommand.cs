using Application.Abstraction.Messaging;
using Application.DTOs.Employee;

namespace Application.Employees.Commands.ChangeAddress
{
    public record UpdateEmployeeAddressCommand(UpdateEmployeeAddressDto UpdateEmployeeAddressDto) : ICommand;
}
