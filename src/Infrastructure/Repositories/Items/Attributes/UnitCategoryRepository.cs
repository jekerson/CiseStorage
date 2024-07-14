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
    public class UnitCategoryRepository : IUnitCategoryRepository
    {
        private readonly SiceDbContext _dbContext;
        private readonly ICacheProvider _cacheProvider;
        private const string UnitCategoriesCacheKey = "unitCategoriesCache";
        private const string UnitCategoryCacheKeyPrefix = "unitCategoryCache_";

        public UnitCategoryRepository(SiceDbContext dbContext, ICacheProvider cacheProvider)
        {
            _dbContext = dbContext;
            _cacheProvider = cacheProvider;
        }

        public async Task<Result<IEnumerable<UnitCategory>>> GetAllUnitCategoriesAsync()
        {
            var unitCategories = await _cacheProvider.GetAsync<IEnumerable<UnitCategory>>(UnitCategoriesCacheKey);
            if (unitCategories == null)
            {
                unitCategories = await _dbContext.UnitCategories.AsNoTracking().ToListAsync();
                await _cacheProvider.SetAsync(UnitCategoriesCacheKey, unitCategories, TimeSpan.FromHours(1));
            }
            return Result<IEnumerable<UnitCategory>>.Success(unitCategories);
        }

        public async Task<Result> AddUnitCategoryAsync(UnitCategory unitCategory)
        {
            if (await IsUnitCategoryExistByNameAsync(unitCategory.Name))
                return Result.Failure(UnitCategoryErrors.UnitCategoryAlreadyExist(unitCategory.Name));

            await _dbContext.UnitCategories.AddAsync(unitCategory);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(UnitCategoriesCacheKey);
            return Result.Success();
        }

        public async Task<Result<UnitCategory>> GetUnitCategoryByIdAsync(int id)
        {
            var cacheKey = $"{UnitCategoryCacheKeyPrefix}{id}";
            var unitCategory = await _cacheProvider.GetAsync<UnitCategory>(cacheKey);
            if (unitCategory == null)
            {
                unitCategory = await _dbContext.UnitCategories.AsNoTracking().FirstOrDefaultAsync(uc => uc.Id == id);
                if (unitCategory == null)
                    return Result<UnitCategory>.Failure(UnitCategoryErrors.UnitCategoryNotFoundById(id));

                await _cacheProvider.SetAsync(cacheKey, unitCategory, TimeSpan.FromHours(1));
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
            await _cacheProvider.RemoveAsync(UnitCategoriesCacheKey);
            await _cacheProvider.RemoveAsync($"{UnitCategoryCacheKeyPrefix}{unitCategory.Id}");
            return Result.Success();
        }

        public async Task<Result> DeleteUnitCategoryAsync(int id)
        {
            var unitCategory = await _dbContext.UnitCategories.FindAsync(id);
            if (unitCategory == null)
                return Result.Failure(UnitCategoryErrors.UnitCategoryNotFoundById(id));

            _dbContext.UnitCategories.Remove(unitCategory);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(UnitCategoriesCacheKey);
            await _cacheProvider.RemoveAsync($"{UnitCategoryCacheKeyPrefix}{id}");
            return Result.Success();
        }

        private async Task<bool> IsUnitCategoryExistByNameAsync(string name)
        {
            return await _dbContext.UnitCategories.AnyAsync(uc => uc.Name == name);
        }
    }
}
