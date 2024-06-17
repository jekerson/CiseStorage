using Domain.Abstraction;
using Domain.Entities;
using Domain.Errors.Items.Attributes;
using Domain.Repositories.Item.Attributes;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Repositories.Items.Attributes
{
    public class AttributeValueTypeRepository : IAttributeValueTypeRepository
    {
        private readonly SiceDbContext _dbContext;
        private readonly IMemoryCache _cache;
        private const string AttributeValueTypesCacheKey = "attributeValueTypesCache";
        private const string AttributeValueTypeCacheKeyPrefix = "attributeValueTypeCache_";

        public AttributeValueTypeRepository(SiceDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<Result<IEnumerable<AttributeValueType>>> GetAllAttributeValueTypesAsync()
        {
            if (!_cache.TryGetValue(AttributeValueTypesCacheKey, out IEnumerable<AttributeValueType> attributeValueTypes))
            {
                attributeValueTypes = await _dbContext.AttributeValueTypes.AsNoTracking().ToListAsync();
                _cache.Set(AttributeValueTypesCacheKey, attributeValueTypes, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1)));
            }
            return Result<IEnumerable<AttributeValueType>>.Success(attributeValueTypes);
        }

        public async Task<Result> AddAttributeValueTypeAsync(AttributeValueType attributeValueType)
        {
            if (await IsAttributeValueTypeExistByNameAsync(attributeValueType.Name))
                return Result.Failure(AttributeValueTypeErrors.AttributeValueTypeAlreadyExist(attributeValueType.Name));

            await _dbContext.AttributeValueTypes.AddAsync(attributeValueType);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(AttributeValueTypesCacheKey);
            return Result.Success();
        }

        public async Task<Result<AttributeValueType>> GetAttributeValueTypeByIdAsync(int id)
        {
            var cacheKey = $"{AttributeValueTypeCacheKeyPrefix}{id}";
            if (!_cache.TryGetValue(cacheKey, out AttributeValueType attributeValueType))
            {
                attributeValueType = await _dbContext.AttributeValueTypes.AsNoTracking().FirstOrDefaultAsync(avt => avt.Id == id);
                if (attributeValueType == null)
                    return Result<AttributeValueType>.Failure(AttributeValueTypeErrors.AttributeValueTypeNotFoundById(id));

                _cache.Set(cacheKey, attributeValueType, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1)));
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
            _cache.Remove(AttributeValueTypesCacheKey);
            _cache.Remove($"{AttributeValueTypeCacheKeyPrefix}{attributeValueType.Id}");
            return Result.Success();
        }

        public async Task<Result> DeleteAttributeValueTypeAsync(int id)
        {
            var attributeValueType = await _dbContext.AttributeValueTypes.FindAsync(id);
            if (attributeValueType == null)
                return Result.Failure(AttributeValueTypeErrors.AttributeValueTypeNotFoundById(id));

            _dbContext.AttributeValueTypes.Remove(attributeValueType);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(AttributeValueTypesCacheKey);
            _cache.Remove($"{AttributeValueTypeCacheKeyPrefix}{id}");
            return Result.Success();
        }

        private async Task<bool> IsAttributeValueTypeExistByNameAsync(string name)
        {
            return await _dbContext.AttributeValueTypes.AnyAsync(avt => avt.Name == name);
        }
    }
}
