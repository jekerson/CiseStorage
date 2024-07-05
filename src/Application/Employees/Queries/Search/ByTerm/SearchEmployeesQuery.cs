using Application.Abstraction.Messaging;
using Application.Abstraction.Pagging;

namespace Application.Employees.Queries.Search.ByTerm
{
    public sealed record SearchEmployeesQuery(
        string? Name,
        string? Surname,
        string? Lastname,
        string? Phone,
        string? Email,
        int PageNumber,
        PageSizeType PageSize
    ) : IQuery<SearchEmployeeResponse>;
}
