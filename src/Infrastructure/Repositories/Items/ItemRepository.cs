using Domain.Abstraction;
using Domain.Entities;
using Domain.Errors.Items;
using Domain.Repositories.Item;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Application.Abstraction.Cache;

namespace Infrastructure.Repositories.Items
{
    public class ItemRepository : IItemRepository
    {
        private readonly SiceDbContext _dbContext;
        private readonly ICacheProvider _cacheProvider;
        private const string ItemsCacheKey = "itemsCache";
        private const string ItemCacheKeyPrefix = "itemCache_";

        public ItemRepository(SiceDbContext dbContext, ICacheProvider cacheProvider)
        {
            _dbContext = dbContext;
            _cacheProvider = cacheProvider;
        }

        public async Task<Result<IEnumerable<Item>>> GetAllItemsAsync()
        {
            var items = await _cacheProvider.GetAsync<IEnumerable<Item>>(ItemsCacheKey);
            if (items == null)
            {
                items = await _dbContext.Items.AsNoTracking().Where(i => !i.IsDeleted).ToListAsync();
                await _cacheProvider.SetAsync(ItemsCacheKey, items, TimeSpan.FromHours(1));
            }
            return Result<IEnumerable<Item>>.Success(items);
        }

        public async Task<Result> AddItemAsync(Item item)
        {
            if (await IsItemExistByNameAsync(item.Name))
                return Result.Failure(ItemErrors.ItemAlreadyExistByName(item.Name));

            if (await IsItemExistByNumberAsync(item.Number))
                return Result.Failure(ItemErrors.ItemAlreadyExistByNumber(item.Number));

            await _dbContext.Items.AddAsync(item);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(ItemsCacheKey);
            return Result.Success();
        }

        public async Task<Result<Item>> GetItemByIdAsync(int id)
        {
            var cacheKey = $"{ItemCacheKeyPrefix}{id}";
            var item = await _cacheProvider.GetAsync<Item>(cacheKey);
            if (item == null)
            {
                item = await _dbContext.Items.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
                if (item == null)
                    return Result<Item>.Failure(ItemErrors.ItemNotFoundById(id));

                await _cacheProvider.SetAsync(cacheKey, item, TimeSpan.FromHours(1));
            }
            return Result<Item>.Success(item);
        }

        public async Task<Result<Item>> GetItemByNameAsync(string name)
        {
            var item = await _dbContext.Items.AsNoTracking().FirstOrDefaultAsync(i => i.Name == name && !i.IsDeleted);
            if (item == null)
                return Result<Item>.Failure(ItemErrors.ItemNotFoundByName(name));

            return Result<Item>.Success(item);
        }

        public async Task<Result<Item>> GetItemByNumberAsync(string number)
        {
            var item = await _dbContext.Items.AsNoTracking().FirstOrDefaultAsync(i => i.Number == number && !i.IsDeleted);
            if (item == null)
                return Result<Item>.Failure(ItemErrors.ItemNotFoundByNumber(number));

            return Result<Item>.Success(item);
        }

        public async Task<Result> UpdateItemAsync(Item item)
        {
            var existingItem = await _dbContext.Items.FindAsync(item.Id);
            if (existingItem == null)
                return Result.Failure(ItemErrors.ItemNotFoundById(item.Id));

            if (existingItem.Name != item.Name && await IsItemExistByNameAsync(item.Name))
                return Result.Failure(ItemErrors.ItemAlreadyExistByName(item.Name));

            if (existingItem.Number != item.Number && await IsItemExistByNumberAsync(item.Number))
                return Result.Failure(ItemErrors.ItemAlreadyExistByNumber(item.Number));

            _dbContext.Entry(existingItem).CurrentValues.SetValues(item);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(ItemsCacheKey);
            await _cacheProvider.RemoveAsync($"{ItemCacheKeyPrefix}{item.Id}");
            return Result.Success();
        }

        public async Task<Result> DeleteItemAsync(int id)
        {
            var item = await _dbContext.Items.FindAsync(id);
            if (item == null)
                return Result.Failure(ItemErrors.ItemNotFoundById(id));

            item.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(ItemsCacheKey);
            await _cacheProvider.RemoveAsync($"{ItemCacheKeyPrefix}{id}");
            return Result.Success();
        }

        public async Task<Result<IEnumerable<Item>>> GetItemsByCategoryIdAsync(int categoryId)
        {
            var items = await _dbContext.Items.AsNoTracking().Where(i => i.CategoryId == categoryId && !i.IsDeleted).ToListAsync();
            return Result<IEnumerable<Item>>.Success(items);
        }

        private async Task<bool> IsItemExistByNameAsync(string name)
        {
            return await _dbContext.Items.AnyAsync(i => i.Name == name && !i.IsDeleted);
        }

        private async Task<bool> IsItemExistByNumberAsync(string number)
        {
            return await _dbContext.Items.AnyAsync(i => i.Number == number && !i.IsDeleted);
        }
    }
}
