using Domain.Abstraction;
using Domain.Entities;

namespace Domain.Repositories.Item.Attributes
{
    public interface IAttributeValueTypeRepository
    {
        Task<Result<IEnumerable<AttributeValueType>>> GetAllAttributeValueTypesAsync();
        Task<Result> AddAttributeValueTypeAsync(AttributeValueType attributeValueType);
        Task<Result<AttributeValueType>> GetAttributeValueTypeByIdAsync(int id);
        Task<Result<AttributeValueType>> GetAttributeValueTypeByNameAsync(string name);
        Task<Result> UpdateAttributeValueTypeAsync(AttributeValueType attributeValueType);
        Task<Result> DeleteAttributeValueTypeAsync(int id);
    }
}
