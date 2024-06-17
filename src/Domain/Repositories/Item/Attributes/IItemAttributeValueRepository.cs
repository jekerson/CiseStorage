using Domain.Abstraction;
using Domain.Entities;

namespace Domain.Repositories.Item.Attributes
{
    public interface IItemAttributeValueRepository
    {
        Task<Result<IEnumerable<ItemAttributeValue>>> GetAllItemAttributeValuesAsync();
        Task<Result> AddItemAttributeValueAsync(ItemAttributeValue itemAttributeValue);
        Task<Result<ItemAttributeValue>> GetItemAttributeValueByIdAsync(int id);
        Task<Result<IEnumerable<ItemAttributeValue>>> GetItemAttributeValuesByItemIdAsync(int itemId);
        Task<Result<IEnumerable<ItemAttributeValue>>> GetItemAttributeValuesByAttributeIdAsync(int attributeId);
        Task<Result> UpdateItemAttributeValueAsync(ItemAttributeValue itemAttributeValue);
        Task<Result> DeleteItemAttributeValueAsync(int id);
    }
}
