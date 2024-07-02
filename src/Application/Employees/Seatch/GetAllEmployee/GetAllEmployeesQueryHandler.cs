using Application.Abstraction.Messaging;
using Domain.Abstraction;
using Domain.Entities;
using Domain.Repositories.Staff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Employees.Seatch.GetAllEmployee
{
    public sealed class GetAllEmployeesQueryHandler : IQueryHandler<GetAllEmployeesQuery, IEnumerable<Employee>>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public GetAllEmployeesQueryHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<Result<IEnumerable<Employee>>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
        {
            return await _employeeRepository.GetAllEmployeesAsync();
        }
    }
}
