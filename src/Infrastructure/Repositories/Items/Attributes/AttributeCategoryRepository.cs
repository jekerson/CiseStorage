using Application.Abstraction.Cache;
using Domain.Abstraction;
using Domain.Entities;
using Domain.Errors.Items.Attributes;
using Domain.Repositories.Item.Attributes;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Items.Attributes
{
    public class AttributeCategoryRepository : IAttributeCategoryRepository
    {
        private readonly SiceDbContext _dbContext;
        private readonly ICacheProvider _cacheProvider;
        private const string AttributeCategoriesCacheKey = "attributeCategoriesCache";
        private const string AttributeCategoryCacheKeyPrefix = "attributeCategoryCache_";

        public AttributeCategoryRepository(SiceDbContext dbContext, ICacheProvider cacheProvider)
        {
            _dbContext = dbContext;
            _cacheProvider = cacheProvider;
        }

        public async Task<Result<IEnumerable<AttributeCategory>>> GetAllAttributeCategoriesAsync()
        {
            var attributeCategories = await _cacheProvider.GetAsync<IEnumerable<AttributeCategory>>(AttributeCategoriesCacheKey);
            if (attributeCategories == null)
            {
                attributeCategories = await _dbContext.AttributeCategories.AsNoTracking().ToListAsync();
                await _cacheProvider.SetAsync(AttributeCategoriesCacheKey, attributeCategories, TimeSpan.FromHours(1));
            }
            return Result<IEnumerable<AttributeCategory>>.Success(attributeCategories);
        }

        public async Task<Result> AddAttributeCategoryAsync(AttributeCategory attributeCategory)
        {
            if (await IsAttributeCategoryExistByNameAsync(attributeCategory.Name))
                return Result.Failure(AttributeCategoryErrors.AttributeCategoryAlreadyExist(attributeCategory.Name));

            await _dbContext.AttributeCategories.AddAsync(attributeCategory);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(AttributeCategoriesCacheKey);
            return Result.Success();
        }

        public async Task<Result<AttributeCategory>> GetAttributeCategoryByIdAsync(int id)
        {
            var cacheKey = $"{AttributeCategoryCacheKeyPrefix}{id}";
            var attributeCategory = await _cacheProvider.GetAsync<AttributeCategory>(cacheKey);
            if (attributeCategory == null)
            {
                attributeCategory = await _dbContext.AttributeCategories.AsNoTracking().FirstOrDefaultAsync(ac => ac.Id == id);
                if (attributeCategory == null)
                    return Result<AttributeCategory>.Failure(AttributeCategoryErrors.AttributeCategoryNotFoundById(id));

                await _cacheProvider.SetAsync(cacheKey, attributeCategory, TimeSpan.FromHours(1));
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
            await _cacheProvider.RemoveAsync(AttributeCategoriesCacheKey);
            await _cacheProvider.RemoveAsync($"{AttributeCategoryCacheKeyPrefix}{attributeCategory.Id}");
            return Result.Success();
        }

        public async Task<Result> DeleteAttributeCategoryAsync(int id)
        {
            var attributeCategory = await _dbContext.AttributeCategories.FindAsync(id);
            if (attributeCategory == null)
                return Result.Failure(AttributeCategoryErrors.AttributeCategoryNotFoundById(id));

            _dbContext.AttributeCategories.Remove(attributeCategory);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(AttributeCategoriesCacheKey);
            await _cacheProvider.RemoveAsync($"{AttributeCategoryCacheKeyPrefix}{id}");
            return Result.Success();
        }

        private async Task<bool> IsAttributeCategoryExistByNameAsync(string name)
        {
            return await _dbContext.AttributeCategories.AnyAsync(ac => ac.Name == name);
        }
    }
}
