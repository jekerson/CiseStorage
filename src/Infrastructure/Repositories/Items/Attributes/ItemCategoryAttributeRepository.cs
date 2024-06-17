using Domain.Abstraction;
using Domain.Entities;
using Domain.Errors.Items.Attributes;
using Domain.Repositories.Item.Attributes;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Repositories.Items.Attributes
{
    public class ItemCategoryAttributeRepository : IItemCategoryAttributeRepository
    {
        private readonly SiceDbContext _dbContext;
        private readonly IMemoryCache _cache;
        private const string ItemCategoryAttributesCacheKey = "itemCategoryAttributesCache";
        private const string ItemCategoryAttributeCacheKeyPrefix = "itemCategoryAttributeCache_";

        public ItemCategoryAttributeRepository(SiceDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<Result<IEnumerable<ItemCategoryAttribute>>> GetAllItemCategoryAttributesAsync()
        {
            if (!_cache.TryGetValue(ItemCategoryAttributesCacheKey, out IEnumerable<ItemCategoryAttribute> itemCategoryAttributes))
            {
                itemCategoryAttributes = await _dbContext.ItemCategoryAttributes.AsNoTracking().ToListAsync();
                _cache.Set(ItemCategoryAttributesCacheKey, itemCategoryAttributes, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1)));
            }
            return Result<IEnumerable<ItemCategoryAttribute>>.Success(itemCategoryAttributes);
        }

        public async Task<Result> AddItemCategoryAttributeAsync(ItemCategoryAttribute itemCategoryAttribute)
        {
            await _dbContext.ItemCategoryAttributes.AddAsync(itemCategoryAttribute);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(ItemCategoryAttributesCacheKey);
            return Result.Success();
        }

        public async Task<Result<ItemCategoryAttribute>> GetItemCategoryAttributeByIdAsync(int id)
        {
            var cacheKey = $"{ItemCategoryAttributeCacheKeyPrefix}{id}";
            if (!_cache.TryGetValue(cacheKey, out ItemCategoryAttribute itemCategoryAttribute))
            {
                itemCategoryAttribute = await _dbContext.ItemCategoryAttributes.AsNoTracking().FirstOrDefaultAsync(ica => ica.Id == id);
                if (itemCategoryAttribute == null)
                    return Result<ItemCategoryAttribute>.Failure(ItemCategoryAttributeErrors.ItemCategoryAttributeNotFoundById(id));

                _cache.Set(cacheKey, itemCategoryAttribute, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1)));
            }
            return Result<ItemCategoryAttribute>.Success(itemCategoryAttribute);
        }

        public async Task<Result<IEnumerable<ItemCategoryAttribute>>> GetItemCategoryAttributesByCategoryIdAsync(int categoryId)
        {
            var itemCategoryAttributes = await _dbContext.ItemCategoryAttributes.AsNoTracking()
                .Where(ica => ica.ItemCategoryId == categoryId)
                .ToListAsync();
            return Result<IEnumerable<ItemCategoryAttribute>>.Success(itemCategoryAttributes);
        }

        public async Task<Result<IEnumerable<ItemCategoryAttribute>>> GetItemCategoryAttributesByAttributeIdAsync(int attributeId)
        {
            var itemCategoryAttributes = await _dbContext.ItemCategoryAttributes.AsNoTracking()
                .Where(ica => ica.AttributeId == attributeId)
                .ToListAsync();
            return Result<IEnumerable<ItemCategoryAttribute>>.Success(itemCategoryAttributes);
        }

        public async Task<Result> UpdateItemCategoryAttributeAsync(ItemCategoryAttribute itemCategoryAttribute)
        {
            var existingItemCategoryAttribute = await _dbContext.ItemCategoryAttributes.FindAsync(itemCategoryAttribute.Id);
            if (existingItemCategoryAttribute == null)
                return Result.Failure(ItemCategoryAttributeErrors.ItemCategoryAttributeNotFoundById(itemCategoryAttribute.Id));

            _dbContext.Entry(existingItemCategoryAttribute).CurrentValues.SetValues(itemCategoryAttribute);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(ItemCategoryAttributesCacheKey);
            _cache.Remove($"{ItemCategoryAttributeCacheKeyPrefix}{itemCategoryAttribute.Id}");
            return Result.Success();
        }

        public async Task<Result> DeleteItemCategoryAttributeAsync(int id)
        {
            var itemCategoryAttribute = await _dbContext.ItemCategoryAttributes.FindAsync(id);
            if (itemCategoryAttribute == null)
                return Result.Failure(ItemCategoryAttributeErrors.ItemCategoryAttributeNotFoundById(id));

            _dbContext.ItemCategoryAttributes.Remove(itemCategoryAttribute);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(ItemCategoryAttributesCacheKey);
            _cache.Remove($"{ItemCategoryAttributeCacheKeyPrefix}{id}");
            return Result.Success();
        }
    }
}
