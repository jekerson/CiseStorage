using Domain.Abstraction;
using Domain.Entities;

namespace Domain.Repositories.Audit
{
    public interface IUserAuditRepository
    {
        Task<Result<IEnumerable<UserInfoAudit>>> GetAllUserAuditsAsync();
        Task<Result> AddUserAuditAsync(UserInfoAudit userAudit);
        Task<Result<UserInfoAudit>> GetUserAuditByIdAsync(int id);
        Task<Result<IEnumerable<UserInfoAudit>>> GetUserAuditsByUserIdAsync(int userId);
        Task<Result<IEnumerable<UserInfoAudit>>> GetUserAuditsByEmployeeIdAsync(int employeeId);
        Task<Result> UpdateUserAuditAsync(UserInfoAudit userAudit);
        Task<Result> DeleteUserAuditAsync(int id);
    }
}
