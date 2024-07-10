using Domain.Abstraction;
using Domain.Entities;
using Domain.Enum;
using Domain.Extensions;
using Domain.Repositories.Audit;
using Domain.Repositories.Staff;

namespace Application.Services.Audit.Staff
{
    public class EmployeeAuditService : IEmployeeAuditService
    {
        private readonly IEmployeeAuditRepository _employeeAuditRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUserRepository _userRepository;

        public EmployeeAuditService(
             IEmployeeAuditRepository employeeAuditRepository,
             IEmployeeRepository employeeRepository,
             IUserRepository userRepository)
        {
            _employeeAuditRepository = employeeAuditRepository;
            _employeeRepository = employeeRepository;
            _userRepository = userRepository;
        }

        public async Task<Result> AddEmployeeAuditAsync(ActionType actionType, int changedById, int employeeId)
        {
            var employeeResult = await GetEmployeeResultAsync(actionType, employeeId);
            if (!employeeResult.IsSuccess)
                return Result.Failure(employeeResult.Error);

            var employee = employeeResult.Value;

            if (actionType == ActionType.Update && !await HasEmployeeChanged(employeeId, employee))
                return Result.Success(); // No changes, no audit record needed

            var employeeAudit = CreateEmployeeAudit(actionType, changedById, employeeId, employee);
            return await _employeeAuditRepository.AddEmployeeAuditAsync(employeeAudit);
        }

        private async Task<Result<Employee>> GetEmployeeResultAsync(ActionType actionType, int employeeId)
        {
            return actionType == ActionType.Delete
                ? await _employeeRepository.GetDeletedEmployeeByIdAsync(employeeId)
                : await _employeeRepository.GetEmployeeByIdAsync(employeeId);
        }

        public async Task<Result<IEnumerable<EmployeeAudit>>> GetEmployeeAuditHistoryAsync(int employeeId)
        {
            return await _employeeAuditRepository.GetEmployeeAuditsByEmployeeIdAsync(employeeId);
        }

        private async Task<bool> HasEmployeeChanged(int employeeId, Employee employee)
        {
            var existingAudit = await _employeeAuditRepository.GetEmployeeAuditByIdAsync(employeeId);
            if (existingAudit.IsSuccess && existingAudit.Value != null)
            {
                var lastAudit = existingAudit.Value;
                return !(lastAudit.Name == employee.Name &&
                         lastAudit.Surname == employee.Surname &&
                         lastAudit.Lastname == employee.Lastname &&
                         lastAudit.Sex == employee.Sex &&
                         lastAudit.Age == employee.Age &&
                         lastAudit.PhoneNumber == employee.PhoneNumber &&
                         lastAudit.EmailAddress == employee.EmailAddress &&
                         lastAudit.AddressId == employee.AddressId &&
                         lastAudit.PositionId == employee.PositionId);
            }
            return true;
        }

        private EmployeeAudit CreateEmployeeAudit(ActionType actionType, int changedById, int employeeId, Employee employee)
        {
            return new EmployeeAudit
            {
                EmployeeId = employeeId,
                Name = employee.Name,
                Surname = employee.Surname,
                Lastname = employee.Lastname,
                Sex = employee.Sex,
                Age = employee.Age,
                PhoneNumber = employee.PhoneNumber,
                EmailAddress = employee.EmailAddress,
                AddressId = employee.AddressId,
                PositionId = employee.PositionId,
                ChangedBy = changedById,
                Action = actionType.GetActionName()
            };
        }
    }
}