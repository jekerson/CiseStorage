using Domain.Abstraction;
using Domain.Entities;

namespace Domain.Repositories.Audit
{
    public interface IItemResponsibilityAuditRepository
    {
        Task<Result<IEnumerable<ItemResponsibilityAudit>>> GetAllItemResponsibilityAuditsAsync();
        Task<Result> AddItemResponsibilityAuditAsync(ItemResponsibilityAudit itemResponsibilityAudit);
        Task<Result<ItemResponsibilityAudit>> GetItemResponsibilityAuditByIdAsync(int id);
        Task<Result<IEnumerable<ItemResponsibilityAudit>>> GetItemResponsibilityAuditsByItemIdAsync(int itemId);
        Task<Result<IEnumerable<ItemResponsibilityAudit>>> GetItemResponsibilityAuditsByChangedByAsync(int userInfoId);
        Task<Result<IEnumerable<ItemResponsibilityAudit>>> GetItemResponsibilityAuditsByEmployeeIdAsync(int employeeId);
        Task<Result> UpdateItemResponsibilityAuditAsync(ItemResponsibilityAudit itemResponsibilityAudit);
        Task<Result> DeleteItemResponsibilityAuditAsync(int id);
    }
}
