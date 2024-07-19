using Application.Abstraction.Cache;
using Domain.Abstraction;
using Domain.Entities;
using Domain.Errors.Items.Attributes;
using Domain.Repositories.Item.Attributes;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Items.Attributes
{
    public class AttributeValueTypeRepository : IAttributeValueTypeRepository
    {
        private readonly SiceDbContext _dbContext;
        private readonly ICacheProvider _cacheProvider;
        private const string AttributeValueTypesCacheKey = "attributeValueTypesCache";
        private const string AttributeValueTypeCacheKeyPrefix = "attributeValueTypeCache_";

        public AttributeValueTypeRepository(SiceDbContext dbContext, ICacheProvider cacheProvider)
        {
            _dbContext = dbContext;
            _cacheProvider = cacheProvider;
        }

        public async Task<Result<IEnumerable<AttributeValueType>>> GetAllAttributeValueTypesAsync()
        {
            var attributeValueTypes = await _cacheProvider.GetAsync<IEnumerable<AttributeValueType>>(AttributeValueTypesCacheKey);
            if (attributeValueTypes == null)
            {
                attributeValueTypes = await _dbContext.AttributeValueTypes.AsNoTracking().ToListAsync();
                await _cacheProvider.SetAsync(AttributeValueTypesCacheKey, attributeValueTypes, TimeSpan.FromHours(1));
            }
            return Result<IEnumerable<AttributeValueType>>.Success(attributeValueTypes);
        }

        public async Task<Result> AddAttributeValueTypeAsync(AttributeValueType attributeValueType)
        {
            if (await IsAttributeValueTypeExistByNameAsync(attributeValueType.Name))
                return Result.Failure(AttributeValueTypeErrors.AttributeValueTypeAlreadyExist(attributeValueType.Name));

            await _dbContext.AttributeValueTypes.AddAsync(attributeValueType);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(AttributeValueTypesCacheKey);
            return Result.Success();
        }

        public async Task<Result<AttributeValueType>> GetAttributeValueTypeByIdAsync(int id)
        {
            var cacheKey = $"{AttributeValueTypeCacheKeyPrefix}{id}";
            var attributeValueType = await _cacheProvider.GetAsync<AttributeValueType>(cacheKey);
            if (attributeValueType == null)
            {
                attributeValueType = await _dbContext.AttributeValueTypes.AsNoTracking().FirstOrDefaultAsync(avt => avt.Id == id);
                if (attributeValueType == null)
                    return Result<AttributeValueType>.Failure(AttributeValueTypeErrors.AttributeValueTypeNotFoundById(id));

                await _cacheProvider.SetAsync(cacheKey, attributeValueType, TimeSpan.FromHours(1));
            }
            return Result<AttributeValueType>.Success(attributeValueType);
        }

        public async Task<Result<AttributeValueType>> GetAttributeValueTypeByNameAsync(string name)
        {
            var attributeValueType = await _dbContext.AttributeValueTypes.AsNoTracking().FirstOrDefaultAsync(avt => avt.Name == name);
            if (attributeValueType == null)
                return Result<AttributeValueType>.Failure(AttributeValueTypeErrors.AttributeValueTypeNotFoundByName(name));

            return Result<AttributeValueType>.Success(attributeValueType);
        }

        public async Task<Result> UpdateAttributeValueTypeAsync(AttributeValueType attributeValueType)
        {
            var existingAttributeValueType = await _dbContext.AttributeValueTypes.FindAsync(attributeValueType.Id);
            if (existingAttributeValueType == null)
                return Result.Failure(AttributeValueTypeErrors.AttributeValueTypeNotFoundById(attributeValueType.Id));

            if (existingAttributeValueType.Name != attributeValueType.Name && await IsAttributeValueTypeExistByNameAsync(attributeValueType.Name))
                return Result.Failure(AttributeValueTypeErrors.AttributeValueTypeAlreadyExist(attributeValueType.Name));

            _dbContext.Entry(existingAttributeValueType).CurrentValues.SetValues(attributeValueType);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(AttributeValueTypesCacheKey);
            await _cacheProvider.RemoveAsync($"{AttributeValueTypeCacheKeyPrefix}{attributeValueType.Id}");
            return Result.Success();
        }

        public async Task<Result> DeleteAttributeValueTypeAsync(int id)
        {
            var attributeValueType = await _dbContext.AttributeValueTypes.FindAsync(id);
            if (attributeValueType == null)
                return Result.Failure(AttributeValueTypeErrors.AttributeValueTypeNotFoundById(id));

            _dbContext.AttributeValueTypes.Remove(attributeValueType);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(AttributeValueTypesCacheKey);
            await _cacheProvider.RemoveAsync($"{AttributeValueTypeCacheKeyPrefix}{id}");
            return Result.Success();
        }

        private async Task<bool> IsAttributeValueTypeExistByNameAsync(string name)
        {
            return await _dbContext.AttributeValueTypes.AnyAsync(avt => avt.Name == name);
        }
    }
}
