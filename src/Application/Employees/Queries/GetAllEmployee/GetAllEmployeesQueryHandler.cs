using Application.Abstraction.Messaging;
using Application.Abstraction.Pagging;
using Application.DTOs.Employee;
using Domain.Abstraction;
using Domain.Repositories.Staff;

namespace Application.Employees.Queries.GetAllEmployee
{
    public sealed class GetAllEmployeesQueryHandler : IQueryHandler<GetAllEmployeesQuery, GetAllEmployeeResponse>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public GetAllEmployeesQueryHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<Result<GetAllEmployeeResponse>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
        {
            var employeesResult = await _employeeRepository.GetAllEmployeesWithPositionAsync();
            if (!employeesResult.IsSuccess)
                return Result<GetAllEmployeeResponse>.Failure(employeesResult.Error);

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

            var response = new GetAllEmployeeResponse
            {
                Employees = pagedEmployees,
                TotalPages = pagedEmployees.TotalPages,
                TotalCount = pagedEmployees.TotalCount
            };

            return Result<GetAllEmployeeResponse>.Success(response);
        }
    }
}
