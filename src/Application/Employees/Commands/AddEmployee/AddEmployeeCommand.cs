using Application.DTOs.Employee;
using Domain.Abstraction;
using MediatR;

namespace Application.Employees.Commands.AddEmployee
{
    public record AddEmployeeCommand(AddEmployeeDto EmployeeDto) : IRequest<Result>;

}