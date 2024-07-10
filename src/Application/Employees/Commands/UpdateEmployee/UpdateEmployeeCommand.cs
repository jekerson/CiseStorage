using Application.Abstraction.Messaging;
using Application.DTOs.Employee;

namespace Application.Employees.Commands.UpdateEmployee
{
    public record UpdateEmployeeCommand(UpdateEmployeeDto EmployeeDto) : ICommand;

}