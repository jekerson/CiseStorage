using Application.Abstraction.Cache;
using Domain.Abstraction;
using Domain.Entities;
using Domain.Errors.Items.Attributes;
using Domain.Repositories.Item.Attributes;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Items.Attributes
{
    public class ItemCategoryAttributeRepository : IItemCategoryAttributeRepository
    {
        private readonly SiceDbContext _dbContext;
        private readonly ICacheProvider _cacheProvider;
        private const string ItemCategoryAttributesCacheKey = "itemCategoryAttributesCache";
        private const string ItemCategoryAttributeCacheKeyPrefix = "itemCategoryAttributeCache_";

        public ItemCategoryAttributeRepository(SiceDbContext dbContext, ICacheProvider cacheProvider)
        {
            _dbContext = dbContext;
            _cacheProvider = cacheProvider;
        }

        public async Task<Result<IEnumerable<ItemCategoryAttribute>>> GetAllItemCategoryAttributesAsync()
        {
            var itemCategoryAttributes = await _cacheProvider.GetAsync<IEnumerable<ItemCategoryAttribute>>(ItemCategoryAttributesCacheKey);
            if (itemCategoryAttributes == null)
            {
                itemCategoryAttributes = await _dbContext.ItemCategoryAttributes.AsNoTracking().ToListAsync();
                await _cacheProvider.SetAsync(ItemCategoryAttributesCacheKey, itemCategoryAttributes, TimeSpan.FromHours(1));
            }
            return Result<IEnumerable<ItemCategoryAttribute>>.Success(itemCategoryAttributes);
        }

        public async Task<Result> AddItemCategoryAttributeAsync(ItemCategoryAttribute itemCategoryAttribute)
        {
            await _dbContext.ItemCategoryAttributes.AddAsync(itemCategoryAttribute);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(ItemCategoryAttributesCacheKey);
            return Result.Success();
        }

        public async Task<Result<ItemCategoryAttribute>> GetItemCategoryAttributeByIdAsync(int id)
        {
            var cacheKey = $"{ItemCategoryAttributeCacheKeyPrefix}{id}";
            var itemCategoryAttribute = await _cacheProvider.GetAsync<ItemCategoryAttribute>(cacheKey);
            if (itemCategoryAttribute == null)
            {
                itemCategoryAttribute = await _dbContext.ItemCategoryAttributes.AsNoTracking().FirstOrDefaultAsync(ica => ica.Id == id);
                if (itemCategoryAttribute == null)
                    return Result<ItemCategoryAttribute>.Failure(ItemCategoryAttributeErrors.ItemCategoryAttributeNotFoundById(id));

                await _cacheProvider.SetAsync(cacheKey, itemCategoryAttribute, TimeSpan.FromHours(1));
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
            await _cacheProvider.RemoveAsync(ItemCategoryAttributesCacheKey);
            await _cacheProvider.RemoveAsync($"{ItemCategoryAttributeCacheKeyPrefix}{itemCategoryAttribute.Id}");
            return Result.Success();
        }

        public async Task<Result> DeleteItemCategoryAttributeAsync(int id)
        {
            var itemCategoryAttribute = await _dbContext.ItemCategoryAttributes.FindAsync(id);
            if (itemCategoryAttribute == null)
                return Result.Failure(ItemCategoryAttributeErrors.ItemCategoryAttributeNotFoundById(id));

            _dbContext.ItemCategoryAttributes.Remove(itemCategoryAttribute);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(ItemCategoryAttributesCacheKey);
            await _cacheProvider.RemoveAsync($"{ItemCategoryAttributeCacheKeyPrefix}{id}");
            return Result.Success();
        }
    }
}
