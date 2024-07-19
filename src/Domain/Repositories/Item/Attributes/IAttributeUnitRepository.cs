using Domain.Abstraction;
using Domain.Entities;

namespace Domain.Repositories.Item.Attributes
{
    public interface IAttributeUnitRepository
    {
        Task<Result<IEnumerable<AttributeUnit>>> GetAllAttributeUnitsAsync();
        Task<Result> AddAttributeUnitAsync(AttributeUnit attributeUnit);
        Task<Result<AttributeUnit>> GetAttributeUnitByIdAsync(int id);
        Task<Result<AttributeUnit>> GetAttributeUnitByNameAsync(string name);
        Task<Result<AttributeUnit>> GetAttributeUnitBySymbolAsync(string symbol);
        Task<Result> UpdateAttributeUnitAsync(AttributeUnit attributeUnit);
        Task<Result> DeleteAttributeUnitAsync(int id);

        // Eager Get All and by id with their related entities
        Task<Result<IEnumerable<AttributeUnit>>> GetAllAttributeUnitsWithEntitiesAsync();
        Task<Result<AttributeUnit>> GetAttributeUnitWithEntitiesByIdAsync(int id);

    }
}
