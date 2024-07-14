using Domain.Abstraction;
using Domain.Entities;
using Domain.Errors.Items.Attributes;
using Domain.Repositories.Item.Attributes;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Application.Abstraction.Cache;

namespace Infrastructure.Repositories.Items.Attributes
{
    public class ItemAttributeValueRepository : IItemAttributeValueRepository
    {
        private readonly SiceDbContext _dbContext;
        private readonly ICacheProvider _cacheProvider;
        private const string ItemAttributeValuesCacheKey = "itemAttributeValuesCache";
        private const string ItemAttributeValueCacheKeyPrefix = "itemAttributeValueCache_";

        public ItemAttributeValueRepository(SiceDbContext dbContext, ICacheProvider cacheProvider)
        {
            _dbContext = dbContext;
            _cacheProvider = cacheProvider;
        }

        public async Task<Result<IEnumerable<ItemAttributeValue>>> GetAllItemAttributeValuesAsync()
        {
            var itemAttributeValues = await _cacheProvider.GetAsync<IEnumerable<ItemAttributeValue>>(ItemAttributeValuesCacheKey);
            if (itemAttributeValues == null)
            {
                itemAttributeValues = await _dbContext.ItemAttributeValues.AsNoTracking().ToListAsync();
                await _cacheProvider.SetAsync(ItemAttributeValuesCacheKey, itemAttributeValues, TimeSpan.FromHours(1));
            }
            return Result<IEnumerable<ItemAttributeValue>>.Success(itemAttributeValues);
        }

        public async Task<Result> AddItemAttributeValueAsync(ItemAttributeValue itemAttributeValue)
        {
            await _dbContext.ItemAttributeValues.AddAsync(itemAttributeValue);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(ItemAttributeValuesCacheKey);
            return Result.Success();
        }

        public async Task<Result<ItemAttributeValue>> GetItemAttributeValueByIdAsync(int id)
        {
            var cacheKey = $"{ItemAttributeValueCacheKeyPrefix}{id}";
            var itemAttributeValue = await _cacheProvider.GetAsync<ItemAttributeValue>(cacheKey);
            if (itemAttributeValue == null)
            {
                itemAttributeValue = await _dbContext.ItemAttributeValues.AsNoTracking().FirstOrDefaultAsync(iav => iav.Id == id);
                if (itemAttributeValue == null)
                    return Result<ItemAttributeValue>.Failure(ItemAttributeValueErrors.ItemAttributeValueNotFoundById(id));

                await _cacheProvider.SetAsync(cacheKey, itemAttributeValue, TimeSpan.FromHours(1));
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
            await _cacheProvider.RemoveAsync(ItemAttributeValuesCacheKey);
            await _cacheProvider.RemoveAsync($"{ItemAttributeValueCacheKeyPrefix}{itemAttributeValue.Id}");
            return Result.Success();
        }

        public async Task<Result> DeleteItemAttributeValueAsync(int id)
        {
            var itemAttributeValue = await _dbContext.ItemAttributeValues.FindAsync(id);
            if (itemAttributeValue == null)
                return Result.Failure(ItemAttributeValueErrors.ItemAttributeValueNotFoundById(id));

            _dbContext.ItemAttributeValues.Remove(itemAttributeValue);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(ItemAttributeValuesCacheKey);
            await _cacheProvider.RemoveAsync($"{ItemAttributeValueCacheKeyPrefix}{id}");
            return Result.Success();
        }
    }
}
