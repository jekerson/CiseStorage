using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Application.Employees.Commands.UpdateEmployee
{
    public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
    {
        public UpdateEmployeeCommandValidator()
        {
            RuleFor(x => x.EmployeeDto.Id).GreaterThan(0);
            RuleFor(x => x.EmployeeDto.Name).NotEmpty().MaximumLength(50);
            RuleFor(x => x.EmployeeDto.Surname).NotEmpty().MaximumLength(50);
            RuleFor(x => x.EmployeeDto.Lastname).NotEmpty().MaximumLength(50);
            RuleFor(x => x.EmployeeDto.Phone).NotEmpty().MaximumLength(15);
            RuleFor(x => x.EmployeeDto.Email).NotEmpty().EmailAddress().MaximumLength(100);
            RuleFor(x => x.EmployeeDto.Sex).NotEmpty().MaximumLength(10);
            RuleFor(x => x.EmployeeDto.Age).GreaterThan(0);
        }
    }
}