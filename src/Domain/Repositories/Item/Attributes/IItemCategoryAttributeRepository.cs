using Domain.Abstraction;
using Domain.Entities;

namespace Domain.Repositories.Item.Attributes
{
    public interface IItemCategoryAttributeRepository
    {
        Task<Result<IEnumerable<ItemCategoryAttribute>>> GetAllItemCategoryAttributesAsync();
        Task<Result> AddItemCategoryAttributeAsync(ItemCategoryAttribute itemCategoryAttribute);
        Task<Result<ItemCategoryAttribute>> GetItemCategoryAttributeByIdAsync(int id);
        Task<Result<IEnumerable<ItemCategoryAttribute>>> GetItemCategoryAttributesByCategoryIdAsync(int categoryId);
        Task<Result<IEnumerable<ItemCategoryAttribute>>> GetItemCategoryAttributesByAttributeIdAsync(int attributeId);
        Task<Result> UpdateItemCategoryAttributeAsync(ItemCategoryAttribute itemCategoryAttribute);
        Task<Result> DeleteItemCategoryAttributeAsync(int id);
    }
}
