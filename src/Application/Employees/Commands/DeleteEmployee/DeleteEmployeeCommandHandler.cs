using Application.Abstraction.Messaging;
using Application.Services.Audit.Staff;
using Domain.Abstraction;
using Domain.Enum;
using Domain.Repositories.Staff;

namespace Application.Employees.Commands.DeleteEmployee
{
    public class DeleteEmployeeCommandHandler : ICommandHandler<DeleteEmployeeCommand>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmployeeAuditService _employeeAuditService;

        public DeleteEmployeeCommandHandler(
            IEmployeeRepository employeeRepository,
            IEmployeeAuditService employeeAuditService)
        {
            _employeeRepository = employeeRepository;
            _employeeAuditService = employeeAuditService;
        }

        public async Task<Result> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employeeResult = await _employeeRepository.GetEmployeeByIdAsync(request.EmployeeId);
            if (!employeeResult.IsSuccess)
                return Result.Failure(employeeResult.Error);

            var employee = employeeResult.Value;
            employee.IsDeleted = true;

            var updateResult = await _employeeRepository.UpdateEmployeeAsync(employee);
            if (!updateResult.IsSuccess)
                return Result.Failure(updateResult.Error);

            var auditResult = await _employeeAuditService.AddEmployeeAuditAsync(ActionType.Delete, request.UserId, employee.Id);
            if (!auditResult.IsSuccess)
                return Result.Failure(auditResult.Error);

            return Result.Success();
        }
    }
}