using Application.Abstraction.Messaging;
using Application.Abstraction.Pagging;
using Application.DTOs.Employee;

namespace Application.Employees.Queries.GetAllEmployee
{
    public sealed record GetAllEmployeesQuery(int PageNumber, PageSizeType PageSize) : IQuery<PagedResponse<EmployeeWithoutDetailsDto>>;
}
