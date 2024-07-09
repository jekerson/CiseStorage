using Application.DTOs.Employee;
using Domain.Abstraction;
using MediatR;

namespace Application.Employees.Commands.UpdateEmployee
{
    public record UpdateEmployeeCommand(UpdateEmployeeDto EmployeeDto) : IRequest<Result>;

}