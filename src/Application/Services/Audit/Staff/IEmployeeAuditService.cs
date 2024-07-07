using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Abstraction;
using Domain.Entities;
using Domain.Enum;

namespace Application.Services.Audit.Staff
{
    public interface IEmployeeAuditService
    {
        Task<Result> AddEmployeeAuditAsync(ActionType actionType, int changedById, int employeeId);
        Task<Result<IEnumerable<EmployeeAudit>>> GetEmployeeAuditHistoryAsync(int employeeId);
    }
}