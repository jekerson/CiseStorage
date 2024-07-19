using Application.Abstraction.Cache;
using Domain.Abstraction;
using Domain.Entities;
using Domain.Errors.Items;
using Domain.Repositories.Item;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Items
{
    public class ItemCategoryRepository : IItemCategoryRepository
    {
        private readonly SiceDbContext _dbContext;
        private readonly ICacheProvider _cacheProvider;
        private const string ItemCategoriesCacheKey = "itemCategoriesCache";
        private const string ItemCategoryCacheKeyPrefix = "itemCategoryCache_";

        public ItemCategoryRepository(SiceDbContext dbContext, ICacheProvider cacheProvider)
        {
            _dbContext = dbContext;
            _cacheProvider = cacheProvider;
        }

        public async Task<Result<IEnumerable<ItemCategory>>> GetAllItemCategoriesAsync()
        {
            var itemCategories = await _cacheProvider.GetAsync<IEnumerable<ItemCategory>>(ItemCategoriesCacheKey);
            if (itemCategories == null)
            {
                itemCategories = await _dbContext.ItemCategories.AsNoTracking().ToListAsync();
                await _cacheProvider.SetAsync(ItemCategoriesCacheKey, itemCategories, TimeSpan.FromHours(1));
            }
            return Result<IEnumerable<ItemCategory>>.Success(itemCategories);
        }

        public async Task<Result> AddItemCategoryAsync(ItemCategory itemCategory)
        {
            if (await IsItemCategoryExistByNameAsync(itemCategory.Name))
                return Result.Failure(ItemCategoryErrors.ItemCategoryAlreadyExist(itemCategory.Name));

            await _dbContext.ItemCategories.AddAsync(itemCategory);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(ItemCategoriesCacheKey);
            return Result.Success();
        }

        public async Task<Result<ItemCategory>> GetItemCategoryByIdAsync(int id)
        {
            var cacheKey = $"{ItemCategoryCacheKeyPrefix}{id}";
            var itemCategory = await _cacheProvider.GetAsync<ItemCategory>(cacheKey);
            if (itemCategory == null)
            {
                itemCategory = await _dbContext.ItemCategories.AsNoTracking().FirstOrDefaultAsync(ic => ic.Id == id);
                if (itemCategory == null)
                    return Result<ItemCategory>.Failure(ItemCategoryErrors.ItemCategoryNotFoundById(id));

                await _cacheProvider.SetAsync(cacheKey, itemCategory, TimeSpan.FromHours(1));
            }
            return Result<ItemCategory>.Success(itemCategory);
        }

        public async Task<Result<ItemCategory>> GetItemCategoryByNameAsync(string name)
        {
            var itemCategory = await _dbContext.ItemCategories.AsNoTracking().FirstOrDefaultAsync(ic => ic.Name == name);
            if (itemCategory == null)
                return Result<ItemCategory>.Failure(ItemCategoryErrors.ItemCategoryNotFoundByName(name));

            return Result<ItemCategory>.Success(itemCategory);
        }

        public async Task<Result<IEnumerable<ItemCategory>>> GetItemCategoryAndSubcategoriesByIdAsync(int id)
        {
            var itemCategories = await _dbContext.ItemCategories.AsNoTracking()
                .Where(ic => ic.Id == id || ic.ParentCategoryId == id)
                .ToListAsync();
            return Result<IEnumerable<ItemCategory>>.Success(itemCategories);
        }

        public async Task<Result> UpdateItemCategoryAsync(ItemCategory itemCategory)
        {
            var existingItemCategory = await _dbContext.ItemCategories.FindAsync(itemCategory.Id);
            if (existingItemCategory == null)
                return Result.Failure(ItemCategoryErrors.ItemCategoryNotFoundById(itemCategory.Id));

            if (existingItemCategory.Name != itemCategory.Name && await IsItemCategoryExistByNameAsync(itemCategory.Name))
                return Result.Failure(ItemCategoryErrors.ItemCategoryAlreadyExist(itemCategory.Name));

            _dbContext.Entry(existingItemCategory).CurrentValues.SetValues(itemCategory);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(ItemCategoriesCacheKey);
            await _cacheProvider.RemoveAsync($"{ItemCategoryCacheKeyPrefix}{itemCategory.Id}");
            return Result.Success();
        }

        public async Task<Result> DeleteItemCategoryAsync(int id)
        {
            var itemCategory = await _dbContext.ItemCategories.FindAsync(id);
            if (itemCategory == null)
                return Result.Failure(ItemCategoryErrors.ItemCategoryNotFoundById(id));

            _dbContext.ItemCategories.Remove(itemCategory);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(ItemCategoriesCacheKey);
            await _cacheProvider.RemoveAsync($"{ItemCategoryCacheKeyPrefix}{id}");
            return Result.Success();
        }

        private async Task<bool> IsItemCategoryExistByNameAsync(string name)
        {
            return await _dbContext.ItemCategories.AnyAsync(ic => ic.Name == name);
        }
    }
}
