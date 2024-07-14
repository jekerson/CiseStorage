using Domain.Abstraction;
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
    public class AttributeRepository : IAttributeRepository
    {
        private readonly SiceDbContext _dbContext;
        private readonly ICacheProvider _cacheProvider;
        private const string AttributesCacheKey = "attributesCache";
        private const string AttributeCacheKeyPrefix = "attributeCache_";

        public AttributeRepository(SiceDbContext dbContext, ICacheProvider cacheProvider)
        {
            _dbContext = dbContext;
            _cacheProvider = cacheProvider;
        }

        public async Task<Result<IEnumerable<Domain.Entities.Attribute>>> GetAllAttributesAsync()
        {
            var attributes = await _cacheProvider.GetAsync<IEnumerable<Domain.Entities.Attribute>>(AttributesCacheKey);
            if (attributes == null)
            {
                attributes = await _dbContext.Attributes.AsNoTracking().ToListAsync();
                await _cacheProvider.SetAsync(AttributesCacheKey, attributes, TimeSpan.FromHours(1));
            }
            return Result<IEnumerable<Domain.Entities.Attribute>>.Success(attributes);
        }

        public async Task<Result> AddAttributeAsync(Domain.Entities.Attribute attribute)
        {
            if (await IsAttributeExistByNameAsync(attribute.Name))
                return Result.Failure(AttributeErrors.AttributeAlreadyExist(attribute.Name));

            await _dbContext.Attributes.AddAsync(attribute);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(AttributesCacheKey);
            return Result.Success();
        }

        public async Task<Result<Domain.Entities.Attribute>> GetAttributeByIdAsync(int id)
        {
            var cacheKey = $"{AttributeCacheKeyPrefix}{id}";
            var attribute = await _cacheProvider.GetAsync<Domain.Entities.Attribute>(cacheKey);
            if (attribute == null)
            {
                attribute = await _dbContext.Attributes.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
                if (attribute == null)
                    return Result<Domain.Entities.Attribute>.Failure(AttributeErrors.AttributeNotFoundById(id));

                await _cacheProvider.SetAsync(cacheKey, attribute, TimeSpan.FromHours(1));
            }
            return Result<Domain.Entities.Attribute>.Success(attribute);
        }

        public async Task<Result<Domain.Entities.Attribute>> GetAttributeByNameAsync(string name)
        {
            var attribute = await _dbContext.Attributes.AsNoTracking().FirstOrDefaultAsync(a => a.Name == name);
            if (attribute == null)
                return Result<Domain.Entities.Attribute>.Failure(AttributeErrors.AttributeNotFoundByName(name));

            return Result<Domain.Entities.Attribute>.Success(attribute);
        }

        public async Task<Result<IEnumerable<Domain.Entities.Attribute>>> GetAttributesByCategoryIdAsync(int categoryId)
        {
            var attributes = await _dbContext.Attributes.AsNoTracking()
                .Where(a => a.AttributeCategoryId == categoryId)
                .ToListAsync();
            return Result<IEnumerable<Domain.Entities.Attribute>>.Success(attributes);
        }

        public async Task<Result<IEnumerable<Domain.Entities.Attribute>>> GetAttributesByUnitIdAsync(int unitId)
        {
            var attributes = await _dbContext.Attributes.AsNoTracking()
                .Where(a => a.AttributeUnitId == unitId)
                .ToListAsync();
            return Result<IEnumerable<Domain.Entities.Attribute>>.Success(attributes);
        }

        public async Task<Result> UpdateAttributeAsync(Domain.Entities.Attribute attribute)
        {
            var existingAttribute = await _dbContext.Attributes.FindAsync(attribute.Id);
            if (existingAttribute == null)
                return Result.Failure(AttributeErrors.AttributeNotFoundById(attribute.Id));

            if (existingAttribute.Name != attribute.Name && await IsAttributeExistByNameAsync(attribute.Name))
                return Result.Failure(AttributeErrors.AttributeAlreadyExist(attribute.Name));

            _dbContext.Entry(existingAttribute).CurrentValues.SetValues(attribute);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(AttributesCacheKey);
            await _cacheProvider.RemoveAsync($"{AttributeCacheKeyPrefix}{attribute.Id}");
            return Result.Success();
        }

        public async Task<Result> DeleteAttributeAsync(int id)
        {
            var attribute = await _dbContext.Attributes.FindAsync(id);
            if (attribute == null)
                return Result.Failure(AttributeErrors.AttributeNotFoundById(id));

            _dbContext.Attributes.Remove(attribute);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(AttributesCacheKey);
            await _cacheProvider.RemoveAsync($"{AttributeCacheKeyPrefix}{id}");
            return Result.Success();
        }

        private async Task<bool> IsAttributeExistByNameAsync(string name)
        {
            return await _dbContext.Attributes.AnyAsync(a => a.Name == name);
        }
    }
}
