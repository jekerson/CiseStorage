using Application.Abstraction.Messaging;
using Application.DTOs.Employee;
using Domain.Entities;

namespace Application.Employees.Queries.Search.ById
{
    public sealed record GetEmployeeByIdQuery(int Id) : IQuery<EmployeeWithDetailsDto>;

}
