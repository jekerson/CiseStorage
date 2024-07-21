using Domain.Abstraction;

namespace Domain.Repositories.Item.Attributes
{
    public interface IAttributeRepository
    {
        Task<Result<IEnumerable<Entities.Attribute>>> GetAllAttributesAsync();
        Task<Result> AddAttributeAsync(Entities.Attribute attribute);
        Task<Result<Entities.Attribute>> GetAttributeByIdAsync(int id);
        Task<Result<Entities.Attribute>> GetAttributeByNameAsync(string name);
        Task<Result<IEnumerable<Entities.Attribute>>> GetAttributesByCategoryIdAsync(int categoryId);
        Task<Result<IEnumerable<Entities.Attribute>>> GetAttributesByUnitIdAsync(int unitId);
        Task<Result> UpdateAttributeAsync(Entities.Attribute attribute);
        Task<Result> DeleteAttributeAsync(int id);

        //Eager
        Task<Result<IEnumerable<Entities.Attribute>>> GetAttributesWithEntitiesByIdsAsync(IEnumerable<int> attributeIds);
        Task<Result<IEnumerable<Entities.Attribute>>> GetAllAttributesWithEntities();

    }
}
