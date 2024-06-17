using Domain.Abstraction;
using Domain.Entities;

namespace Domain.Repositories.Item
{
    public interface IItemResponsibilityRepository
    {
        Task<Result<IEnumerable<ItemResponsibility>>> GetAllItemResponsibilitiesAsync();
        Task<Result> AddItemResponsibilityAsync(ItemResponsibility itemResponsibility);
        Task<Result<ItemResponsibility>> GetItemResponsibilityByIdAsync(int id);
        Task<Result<IEnumerable<ItemResponsibility>>> GetItemResponsibilitiesByItemIdAsync(int itemId);
        Task<Result<IEnumerable<ItemResponsibility>>> GetItemResponsibilitiesByEmployeeIdAsync(int employeeId);
        Task<Result> UpdateItemResponsibilityAsync(ItemResponsibility itemResponsibility);
        Task<Result> DeleteItemResponsibilityAsync(int id);
    }
}
