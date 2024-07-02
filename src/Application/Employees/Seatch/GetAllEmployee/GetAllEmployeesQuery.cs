using Application.Abstraction.Messaging;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Employees.Seatch.GetAllEmployee
{
    public sealed record GetAllEmployeesQuery : IQuery<IEnumerable<Employee>>;

}
