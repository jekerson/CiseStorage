using Domain.Abstraction;
using Domain.Entities;

namespace Domain.Repositories.Audit
{
    public interface IEmployeeAuditRepository
    {
        Task<Result<IEnumerable<EmployeeAudit>>> GetAllEmployeeAuditsAsync();
        Task<Result> AddEmployeeAuditAsync(EmployeeAudit employeeAudit);
        Task<Result<EmployeeAudit>> GetEmployeeAuditByIdAsync(int id);
        Task<Result<IEnumerable<EmployeeAudit>>> GetEmployeeAuditsByEmployeeIdAsync(int employeeId);
        Task<Result> UpdateEmployeeAuditAsync(EmployeeAudit employeeAudit);
        Task<Result> DeleteEmployeeAuditAsync(int id);
    }
}
