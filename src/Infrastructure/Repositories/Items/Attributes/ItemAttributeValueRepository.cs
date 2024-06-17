using Domain.Abstraction;
using Domain.Entities;
using Domain.Errors.Items.Attributes;
using Domain.Repositories.Item.Attributes;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Repositories.Items.Attributes
{
    public class ItemAttributeValueRepository : IItemAttributeValueRepository
    {
        private readonly SiceDbContext _dbContext;
        private readonly IMemoryCache _cache;
        private const string ItemAttributeValuesCacheKey = "itemAttributeValuesCache";
        private const string ItemAttributeValueCacheKeyPrefix = "itemAttributeValueCache_";

        public ItemAttributeValueRepository(SiceDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<Result<IEnumerable<ItemAttributeValue>>> GetAllItemAttributeValuesAsync()
        {
            if (!_cache.TryGetValue(ItemAttributeValuesCacheKey, out IEnumerable<ItemAttributeValue> itemAttributeValues))
            {
                itemAttributeValues = await _dbContext.ItemAttributeValues.AsNoTracking().ToListAsync();
                _cache.Set(ItemAttributeValuesCacheKey, itemAttributeValues, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1)));
            }
            return Result<IEnumerable<ItemAttributeValue>>.Success(itemAttributeValues);
        }

        public async Task<Result> AddItemAttributeValueAsync(ItemAttributeValue itemAttributeValue)
        {
            await _dbContext.ItemAttributeValues.AddAsync(itemAttributeValue);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(ItemAttributeValuesCacheKey);
            return Result.Success();
        }

        public async Task<Result<ItemAttributeValue>> GetItemAttributeValueByIdAsync(int id)
        {
            var cacheKey = $"{ItemAttributeValueCacheKeyPrefix}{id}";
            if (!_cache.TryGetValue(cacheKey, out ItemAttributeValue itemAttributeValue))
            {
                itemAttributeValue = await _dbContext.ItemAttributeValues.AsNoTracking().FirstOrDefaultAsync(iav => iav.Id == id);
                if (itemAttributeValue == null)
                    return Result<ItemAttributeValue>.Failure(ItemAttributeValueErrors.ItemAttributeValueNotFoundById(id));

                _cache.Set(cacheKey, itemAttributeValue, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1)));
            }
            return Result<ItemAttributeValue>.Success(itemAttributeValue);
        }

        public async Task<Result<IEnumerable<ItemAttributeValue>>> GetItemAttributeValuesByItemIdAsync(int itemId)
        {
            var itemAttributeValues = await _dbContext.ItemAttributeValues.AsNoTracking()
                .Where(iav => iav.ItemId == itemId)
                .ToListAsync();
            return Result<IEnumerable<ItemAttributeValue>>.Success(itemAttributeValues);
        }

        public async Task<Result<IEnumerable<ItemAttributeValue>>> GetItemAttributeValuesByAttributeIdAsync(int attributeId)
        {
            var itemAttributeValues = await _dbContext.ItemAttributeValues.AsNoTracking()
                .Where(iav => iav.AttributeId == attributeId)
                .ToListAsync();
            return Result<IEnumerable<ItemAttributeValue>>.Success(itemAttributeValues);
        }

        public async Task<Result> UpdateItemAttributeValueAsync(ItemAttributeValue itemAttributeValue)
        {
            var existingItemAttributeValue = await _dbContext.ItemAttributeValues.FindAsync(itemAttributeValue.Id);
            if (existingItemAttributeValue == null)
                return Result.Failure(ItemAttributeValueErrors.ItemAttributeValueNotFoundById(itemAttributeValue.Id));

            _dbContext.Entry(existingItemAttributeValue).CurrentValues.SetValues(itemAttributeValue);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(ItemAttributeValuesCacheKey);
            _cache.Remove($"{ItemAttributeValueCacheKeyPrefix}{itemAttributeValue.Id}");
            return Result.Success();
        }

        public async Task<Result> DeleteItemAttributeValueAsync(int id)
        {
            var itemAttributeValue = await _dbContext.ItemAttributeValues.FindAsync(id);
            if (itemAttributeValue == null)
                return Result.Failure(ItemAttributeValueErrors.ItemAttributeValueNotFoundById(id));

            _dbContext.ItemAttributeValues.Remove(itemAttributeValue);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(ItemAttributeValuesCacheKey);
            _cache.Remove($"{ItemAttributeValueCacheKeyPrefix}{id}");
            return Result.Success();
        }
    }
}
