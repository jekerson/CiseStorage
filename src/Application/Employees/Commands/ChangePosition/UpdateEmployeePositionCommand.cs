using Application.Abstraction.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Employees.Commands.ChangePosition
{
    public record UpdateEmployeePositionCommand(int EmployeeId, int NewPositionId): ICommand;
}
