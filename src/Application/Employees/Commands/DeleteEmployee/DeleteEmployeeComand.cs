using Application.Abstraction.Messaging;
using Application.DTOs.Employee;

namespace Application.Employees.Commands.DeleteEmployee
{
    public record DeleteEmployeeCommand(DeleteEmployeeDto DeleteEmployeeDto) : ICommand;

}