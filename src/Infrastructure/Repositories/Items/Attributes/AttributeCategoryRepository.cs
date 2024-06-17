using Domain.Abstraction;
using Domain.Entities;
using Domain.Errors.Items.Attributes;
using Domain.Repositories.Item.Attributes;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Repositories.Items.Attributes
{
    public class AttributeCategoryRepository : IAttributeCategoryRepository
    {
        private readonly SiceDbContext _dbContext;
        private readonly IMemoryCache _cache;
        private const string AttributeCategoriesCacheKey = "attributeCategoriesCache";
        private const string AttributeCategoryCacheKeyPrefix = "attributeCategoryCache_";

        public AttributeCategoryRepository(SiceDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<Result<IEnumerable<AttributeCategory>>> GetAllAttributeCategoriesAsync()
        {
            if (!_cache.TryGetValue(AttributeCategoriesCacheKey, out IEnumerable<AttributeCategory> attributeCategories))
            {
                attributeCategories = await _dbContext.AttributeCategories.AsNoTracking().ToListAsync();
                _cache.Set(AttributeCategoriesCacheKey, attributeCategories, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1)));
            }
            return Result<IEnumerable<AttributeCategory>>.Success(attributeCategories);
        }

        public async Task<Result> AddAttributeCategoryAsync(AttributeCategory attributeCategory)
        {
            if (await IsAttributeCategoryExistByNameAsync(attributeCategory.Name))
                return Result.Failure(AttributeCategoryErrors.AttributeCategoryAlreadyExist(attributeCategory.Name));

            await _dbContext.AttributeCategories.AddAsync(attributeCategory);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(AttributeCategoriesCacheKey);
            return Result.Success();
        }

        public async Task<Result<AttributeCategory>> GetAttributeCategoryByIdAsync(int id)
        {
            var cacheKey = $"{AttributeCategoryCacheKeyPrefix}{id}";
            if (!_cache.TryGetValue(cacheKey, out AttributeCategory attributeCategory))
            {
                attributeCategory = await _dbContext.AttributeCategories.AsNoTracking().FirstOrDefaultAsync(ac => ac.Id == id);
                if (attributeCategory == null)
                    return Result<AttributeCategory>.Failure(AttributeCategoryErrors.AttributeCategoryNotFoundById(id));

                _cache.Set(cacheKey, attributeCategory, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1)));
            }
            return Result<AttributeCategory>.Success(attributeCategory);
        }

        public async Task<Result<AttributeCategory>> GetAttributeCategoryByNameAsync(string name)
        {
            var attributeCategory = await _dbContext.AttributeCategories.AsNoTracking().FirstOrDefaultAsync(ac => ac.Name == name);
            if (attributeCategory == null)
                return Result<AttributeCategory>.Failure(AttributeCategoryErrors.AttributeCategoryNotFoundByName(name));

            return Result<AttributeCategory>.Success(attributeCategory);
        }

        public async Task<Result> UpdateAttributeCategoryAsync(AttributeCategory attributeCategory)
        {
            var existingAttributeCategory = await _dbContext.AttributeCategories.FindAsync(attributeCategory.Id);
            if (existingAttributeCategory == null)
                return Result.Failure(AttributeCategoryErrors.AttributeCategoryNotFoundById(attributeCategory.Id));

            if (existingAttributeCategory.Name != attributeCategory.Name && await IsAttributeCategoryExistByNameAsync(attributeCategory.Name))
                return Result.Failure(AttributeCategoryErrors.AttributeCategoryAlreadyExist(attributeCategory.Name));

            _dbContext.Entry(existingAttributeCategory).CurrentValues.SetValues(attributeCategory);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(AttributeCategoriesCacheKey);
            _cache.Remove($"{AttributeCategoryCacheKeyPrefix}{attributeCategory.Id}");
            return Result.Success();
        }

        public async Task<Result> DeleteAttributeCategoryAsync(int id)
        {
            var attributeCategory = await _dbContext.AttributeCategories.FindAsync(id);
            if (attributeCategory == null)
                return Result.Failure(AttributeCategoryErrors.AttributeCategoryNotFoundById(id));

            _dbContext.AttributeCategories.Remove(attributeCategory);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(AttributeCategoriesCacheKey);
            _cache.Remove($"{AttributeCategoryCacheKeyPrefix}{id}");
            return Result.Success();
        }

        private async Task<bool> IsAttributeCategoryExistByNameAsync(string name)
        {
            return await _dbContext.AttributeCategories.AnyAsync(ac => ac.Name == name);
        }
    }
}
