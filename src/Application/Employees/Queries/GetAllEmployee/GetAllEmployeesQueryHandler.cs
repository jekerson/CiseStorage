using Application.Abstraction.Messaging;
using Application.Abstraction.Pagging;
using Application.DTOs.Employee;
using Domain.Abstraction;
using Domain.Repositories.Staff;

namespace Application.Employees.Queries.GetAllEmployee
{
public sealed class GetAllEmployeesQueryHandler : IQueryHandler<GetAllEmployeesQuery, PagedResponse<EmployeeWithoutDetailsDto>>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public GetAllEmployeesQueryHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<Result<PagedResponse<EmployeeWithoutDetailsDto>>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
        {
            var employeesResult = await _employeeRepository.GetAllEmployeesWithPositionAsync();
            if (!employeesResult.IsSuccess)
                return Result<PagedResponse<EmployeeWithoutDetailsDto>>.Failure(employeesResult.Error);

            var pagedEmployees = PagedList<EmployeeWithoutDetailsDto>.ToPagedList(
                employeesResult.Value.Select(e => new EmployeeWithoutDetailsDto(
                    e.Id,
                    e.Name,
                    e.Surname,
                    e.Lastname,
                    e.PhoneNumber,
                    e.Position.Name
                )).AsQueryable(),
                request.PageNumber,
                request.PageSize
            );

            var response = new PagedResponse<EmployeeWithoutDetailsDto>
            {
                Items = pagedEmployees,
                TotalPages = pagedEmployees.TotalPages,
                TotalCount = pagedEmployees.TotalCount
            };

            return Result<PagedResponse<EmployeeWithoutDetailsDto>>.Success(response);
        }
    }
}
