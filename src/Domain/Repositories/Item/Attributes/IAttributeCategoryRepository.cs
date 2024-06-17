using Domain.Abstraction;
using Domain.Entities;

namespace Domain.Repositories.Item.Attributes
{
    public interface IAttributeCategoryRepository
    {
        Task<Result<IEnumerable<AttributeCategory>>> GetAllAttributeCategoriesAsync();
        Task<Result> AddAttributeCategoryAsync(AttributeCategory attributeCategory);
        Task<Result<AttributeCategory>> GetAttributeCategoryByIdAsync(int id);
        Task<Result<AttributeCategory>> GetAttributeCategoryByNameAsync(string name);
        Task<Result> UpdateAttributeCategoryAsync(AttributeCategory attributeCategory);
        Task<Result> DeleteAttributeCategoryAsync(int id);
    }
}
