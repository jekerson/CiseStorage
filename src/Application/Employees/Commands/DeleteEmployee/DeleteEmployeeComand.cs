using Application.Abstraction.Messaging;

namespace Application.Employees.Commands.DeleteEmployee
{
    public record DeleteEmployeeCommand(int EmployeeId, int UserId) : ICommand;

}