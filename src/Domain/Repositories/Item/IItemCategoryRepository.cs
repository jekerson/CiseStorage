using Domain.Abstraction;
using Domain.Entities;

namespace Domain.Repositories.Item
{
    public interface IItemCategoryRepository
    {
        Task<Result<IEnumerable<ItemCategory>>> GetAllItemCategoriesAsync();
        Task<Result> AddItemCategoryAsync(ItemCategory itemCategory);
        Task<Result<ItemCategory>> GetItemCategoryByIdAsync(int id);
        Task<Result<ItemCategory>> GetItemCategoryByNameAsync(string name);
        Task<Result<IEnumerable<ItemCategory>>> GetItemCategoryAndSubcategoriesByIdAsync(int id);
        Task<Result> UpdateItemCategoryAsync(ItemCategory itemCategory);
        Task<Result> DeleteItemCategoryAsync(int id);
    }
}
