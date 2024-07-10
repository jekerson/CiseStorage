using Application.Abstraction.Messaging;
using Application.DTOs.Employee;

namespace Application.Employees.Commands.AddEmployee
{
    public record AddEmployeeCommand(AddEmployeeDto EmployeeDto) : ICommand;

}