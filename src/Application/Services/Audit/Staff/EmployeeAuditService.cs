using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public EmployeeAuditService(IEmployeeAuditRepository employeeAuditRepository, IEmployeeRepository employeeRepository)
    {
        _employeeAuditRepository = employeeAuditRepository;
        _employeeRepository = employeeRepository;
    }

    public async Task<Result> AddEmployeeAuditAsync(ActionType actionType, int changedById, int employeeId)
    {
        var employeeResult = await _employeeRepository.GetEmployeeByIdAsync(employeeId);
        if (!employeeResult.IsSuccess)
            return Result.Failure(employeeResult.Error);

        var employee = employeeResult.Value;

        var employeeAudit = new EmployeeAudit
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

        return await _employeeAuditRepository.AddEmployeeAuditAsync(employeeAudit);
    }

    public async Task<Result<IEnumerable<EmployeeAudit>>> GetEmployeeAuditHistoryAsync(int employeeId)
    {
        return await _employeeAuditRepository.GetEmployeeAuditsByEmployeeIdAsync(employeeId);
    }
}
}