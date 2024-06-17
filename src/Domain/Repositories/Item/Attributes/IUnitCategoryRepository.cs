using Domain.Abstraction;
using Domain.Entities;

namespace Domain.Repositories.Item.Attributes
{
    public interface IUnitCategoryRepository
    {
        Task<Result<IEnumerable<UnitCategory>>> GetAllUnitCategoriesAsync();
        Task<Result> AddUnitCategoryAsync(UnitCategory unitCategory);
        Task<Result<UnitCategory>> GetUnitCategoryByIdAsync(int id);
        Task<Result<UnitCategory>> GetUnitCategoryByNameAsync(string name);
        Task<Result> UpdateUnitCategoryAsync(UnitCategory unitCategory);
        Task<Result> DeleteUnitCategoryAsync(int id);
    }
}
