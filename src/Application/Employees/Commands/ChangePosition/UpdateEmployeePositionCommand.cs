using Application.Abstraction.Messaging;
using Application.DTOs.Employee;

namespace Application.Employees.Commands.ChangePosition
{
    public record UpdateEmployeePositionCommand(UpdateEmployeePositionDto UpdateEmployeePositionDto) : ICommand;
}
