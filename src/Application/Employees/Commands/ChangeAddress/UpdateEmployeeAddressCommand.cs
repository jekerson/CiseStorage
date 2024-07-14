using Application.Abstraction.Messaging;
using Application.DTOs.Addresses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Employees.Commands.ChangeAddress
{
    public record UpdateEmployeeAddressCommand(int EmployeeId, AddAddressDto NewAddress): ICommand;
}
