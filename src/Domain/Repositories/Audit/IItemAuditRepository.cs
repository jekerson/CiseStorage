using Domain.Abstraction;
using Domain.Entities;

namespace Domain.Repositories.Audit
{
    public interface IItemAuditRepository
    {
        Task<Result<IEnumerable<ItemAudit>>> GetAllItemAuditsAsync();
        Task<Result> AddItemAuditAsync(ItemAudit itemAudit);
        Task<Result<ItemAudit>> GetItemAuditByIdAsync(int id);
        Task<Result<IEnumerable<ItemAudit>>> GetItemAuditsByItemIdAsync(int itemId);
        Task<Result<IEnumerable<ItemAudit>>> GetItemAuditsByChangedByUserAsync(int userInfoId);
        Task<Result> UpdateItemAuditAsync(ItemAudit itemAudit);
        Task<Result> DeleteItemAuditAsync(int id);
    }
}
