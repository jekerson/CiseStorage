using Application.Abstraction.Messaging;
using Application.Abstraction.Pagging;

namespace Application.Employees.Queries.GetAllEmployee
{
    public sealed record GetAllEmployeesQuery(int PageNumber, PageSizeType PageSize) : IQuery<GetAllEmployeeResponse>;
}
