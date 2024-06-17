using Domain.Abstraction;
using Domain.Entities;
using Domain.Errors.Items.Attributes;
using Domain.Repositories.Item.Attributes;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Repositories.Items.Attributes
{
    public class UnitCategoryRepository : IUnitCategoryRepository
    {
        private readonly SiceDbContext _dbContext;
        private readonly IMemoryCache _cache;
        private const string UnitCategoriesCacheKey = "unitCategoriesCache";
        private const string UnitCategoryCacheKeyPrefix = "unitCategoryCache_";

        public UnitCategoryRepository(SiceDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<Result<IEnumerable<UnitCategory>>> GetAllUnitCategoriesAsync()
        {
            if (!_cache.TryGetValue(UnitCategoriesCacheKey, out IEnumerable<UnitCategory> unitCategories))
            {
                unitCategories = await _dbContext.UnitCategories.AsNoTracking().ToListAsync();
                _cache.Set(UnitCategoriesCacheKey, unitCategories, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1)));
            }
            return Result<IEnumerable<UnitCategory>>.Success(unitCategories);
        }

        public async Task<Result> AddUnitCategoryAsync(UnitCategory unitCategory)
        {
            if (await IsUnitCategoryExistByNameAsync(unitCategory.Name))
                return Result.Failure(UnitCategoryErrors.UnitCategoryAlreadyExist(unitCategory.Name));

            await _dbContext.UnitCategories.AddAsync(unitCategory);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(UnitCategoriesCacheKey);
            return Result.Success();
        }

        public async Task<Result<UnitCategory>> GetUnitCategoryByIdAsync(int id)
        {
            var cacheKey = $"{UnitCategoryCacheKeyPrefix}{id}";
            if (!_cache.TryGetValue(cacheKey, out UnitCategory unitCategory))
            {
                unitCategory = await _dbContext.UnitCategories.AsNoTracking().FirstOrDefaultAsync(uc => uc.Id == id);
                if (unitCategory == null)
                    return Result<UnitCategory>.Failure(UnitCategoryErrors.UnitCategoryNotFoundById(id));

                _cache.Set(cacheKey, unitCategory, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1)));
            }
            return Result<UnitCategory>.Success(unitCategory);
        }

        public async Task<Result<UnitCategory>> GetUnitCategoryByNameAsync(string name)
        {
            var unitCategory = await _dbContext.UnitCategories.AsNoTracking().FirstOrDefaultAsync(uc => uc.Name == name);
            if (unitCategory == null)
                return Result<UnitCategory>.Failure(UnitCategoryErrors.UnitCategoryNotFoundByName(name));

            return Result<UnitCategory>.Success(unitCategory);
        }

        public async Task<Result> UpdateUnitCategoryAsync(UnitCategory unitCategory)
        {
            var existingUnitCategory = await _dbContext.UnitCategories.FindAsync(unitCategory.Id);
            if (existingUnitCategory == null)
                return Result.Failure(UnitCategoryErrors.UnitCategoryNotFoundById(unitCategory.Id));

            if (existingUnitCategory.Name != unitCategory.Name && await IsUnitCategoryExistByNameAsync(unitCategory.Name))
                return Result.Failure(UnitCategoryErrors.UnitCategoryAlreadyExist(unitCategory.Name));

            _dbContext.Entry(existingUnitCategory).CurrentValues.SetValues(unitCategory);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(UnitCategoriesCacheKey);
            _cache.Remove($"{UnitCategoryCacheKeyPrefix}{unitCategory.Id}");
            return Result.Success();
        }

        public async Task<Result> DeleteUnitCategoryAsync(int id)
        {
            var unitCategory = await _dbContext.UnitCategories.FindAsync(id);
            if (unitCategory == null)
                return Result.Failure(UnitCategoryErrors.UnitCategoryNotFoundById(id));

            _dbContext.UnitCategories.Remove(unitCategory);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(UnitCategoriesCacheKey);
            _cache.Remove($"{UnitCategoryCacheKeyPrefix}{id}");
            return Result.Success();
        }

        private async Task<bool> IsUnitCategoryExistByNameAsync(string name)
        {
            return await _dbContext.UnitCategories.AnyAsync(uc => uc.Name == name);
        }
    }
}
