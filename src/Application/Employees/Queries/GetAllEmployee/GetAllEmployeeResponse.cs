using Application.DTOs.Employee;

namespace Application.Employees.Queries.GetAllEmployee
{
    public sealed record GetAllEmployeeResponse
    {
        public List<EmployeeWithoutDetailsDto> Employees { get; init; }
        public int TotalPages { get; init; }
        public int TotalCount { get; init; }
    }

}
