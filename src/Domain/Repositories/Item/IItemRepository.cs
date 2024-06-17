using Domain.Abstraction;

namespace Domain.Repositories.Item
{
    public interface IItemRepository
    {
        Task<Result<IEnumerable<Entities.Item>>> GetAllItemsAsync();
        Task<Result> AddItemAsync(Entities.Item item);
        Task<Result<Entities.Item>> GetItemByIdAsync(int id);
        Task<Result<Entities.Item>> GetItemByNameAsync(string name);
        Task<Result<Entities.Item>> GetItemByNumberAsync(string number);
        Task<Result> UpdateItemAsync(Entities.Item item);
        Task<Result> DeleteItemAsync(int id);
        Task<Result<IEnumerable<Entities.Item>>> GetItemsByCategoryIdAsync(int categoryId);
    }
}
