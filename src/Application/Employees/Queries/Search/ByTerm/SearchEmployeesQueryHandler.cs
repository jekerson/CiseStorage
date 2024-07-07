using Application.Abstraction.Messaging;
using Application.Abstraction.Pagging;
using Application.DTOs.Employee;
using Domain.Abstraction;
using Domain.Repositories.Staff;

namespace Application.Employees.Queries.Search.ByTerm
{
 public sealed class SearchEmployeesQueryHandler : IQueryHandler<SearchEmployeesQuery, PagedResponse<EmployeeWithoutDetailsDto>>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public SearchEmployeesQueryHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<Result<PagedResponse<EmployeeWithoutDetailsDto>>> Handle(SearchEmployeesQuery request, CancellationToken cancellationToken)
        {
            var employeesResult = await _employeeRepository.GetAllEmployeesWithPositionAsync();
            if (!employeesResult.IsSuccess)
            {
                return Result<PagedResponse<EmployeeWithoutDetailsDto>>.Failure(employeesResult.Error);
            }

            var employees = employeesResult.Value;

            var filteredEmployees = employees
                .Where(e =>
                    (string.IsNullOrEmpty(request.Name) || e.Name.Contains(request.Name)) &&
                    (string.IsNullOrEmpty(request.Surname) || e.Surname.Contains(request.Surname)) &&
                    (string.IsNullOrEmpty(request.Lastname) || e.Lastname.Contains(request.Lastname)) &&
                    (string.IsNullOrEmpty(request.Phone) || e.PhoneNumber.Contains(request.Phone)) &&
                    (string.IsNullOrEmpty(request.Email) || e.EmailAddress.Contains(request.Email))
                )
                .Select(e => new EmployeeWithoutDetailsDto(
                    e.Id,
                    e.Name,
                    e.Surname,
                    e.Lastname,
                    e.PhoneNumber,
                    e.Position.Name
                )).AsQueryable();

            var pagedEmployees = PagedList<EmployeeWithoutDetailsDto>.ToPagedList(filteredEmployees, request.PageNumber, request.PageSize);
            
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
