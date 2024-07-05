using Application.DTOs.Employee;

namespace Application.Employees.Queries.Search.ByTerm
{
    public sealed record SearchEmployeeResponse
    {
        public List<EmployeeWithoutDetailsDto> Employees { get; init; }
        public int TotalPages { get; init; }
        public int TotalCount { get; init; }
    }
}
