using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Services.Audit.Staff;
using Domain.Abstraction;
using Domain.Enum;
using Domain.Repositories.Addresses;
using Domain.Repositories.Staff;
using MediatR;

namespace Application.Employees.Commands.UpdateEmployee
{
    public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, Result>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmployeeAuditService _employeeAuditService;
        private readonly IUserRepository _userRepository;

        public UpdateEmployeeCommandHandler(
            IEmployeeRepository employeeRepository,
            IEmployeeAuditService employeeAuditService,
            IUserRepository userRepository)
        {
            _employeeRepository = employeeRepository;
            _employeeAuditService = employeeAuditService;
            _userRepository = userRepository;
        }

        public async Task<Result> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employeeDto = request.EmployeeDto;

            var userResult = await _userRepository.GetUserByIdAsync(employeeDto.UserId);
            if (!userResult.IsSuccess)
                return Result.Failure(userResult.Error);

            var employeeResult = await _employeeRepository.GetEmployeeByIdAsync(employeeDto.Id);
            if (!employeeResult.IsSuccess)
                return Result.Failure(employeeResult.Error);

            var employee = employeeResult.Value;

            employee.Name = employeeDto.Name;
            employee.Surname = employeeDto.Surname;
            employee.Lastname = employeeDto.Lastname;
            employee.PhoneNumber = employeeDto.Phone;
            employee.EmailAddress = employeeDto.Email;
            employee.Sex = employeeDto.Sex;
            employee.Age = employeeDto.Age;

            var updateResult = await _employeeRepository.UpdateEmployeeAsync(employee);
            if (!updateResult.IsSuccess)
                return Result.Failure(updateResult.Error);

            var auditResult = await _employeeAuditService.AddEmployeeAuditAsync(ActionType.Update, employeeDto.UserId, employee.Id);
            if (!auditResult.IsSuccess)
                return Result.Failure(auditResult.Error);

            return Result.Success();
        }
    }
}