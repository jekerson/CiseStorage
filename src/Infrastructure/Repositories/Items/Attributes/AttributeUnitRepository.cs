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
    public class AttributeUnitRepository : IAttributeUnitRepository
    {
        private readonly SiceDbContext _dbContext;
        private readonly ICacheProvider _cacheProvider;
        private const string AttributeUnitsCacheKey = "attributeUnitsCache";
        private const string AttributeUnitCacheKeyPrefix = "attributeUnitCache_";

        public AttributeUnitRepository(SiceDbContext dbContext, ICacheProvider cacheProvider)
        {
            _dbContext = dbContext;
            _cacheProvider = cacheProvider;
        }

        public async Task<Result<IEnumerable<AttributeUnit>>> GetAllAttributeUnitsAsync()
        {
            var attributeUnits = await _cacheProvider.GetAsync<IEnumerable<AttributeUnit>>(AttributeUnitsCacheKey);
            if (attributeUnits == null)
            {
                attributeUnits = await _dbContext.AttributeUnits.AsNoTracking().ToListAsync();
                await _cacheProvider.SetAsync(AttributeUnitsCacheKey, attributeUnits, TimeSpan.FromHours(1));
            }
            return Result<IEnumerable<AttributeUnit>>.Success(attributeUnits);
        }

        public async Task<Result> AddAttributeUnitAsync(AttributeUnit attributeUnit)
        {
            if (await IsAttributeUnitExistByNameAsync(attributeUnit.Name))
                return Result.Failure(AttributeUnitErrors.AttributeUnitAlreadyExistByName(attributeUnit.Name));

            if (await IsAttributeUnitExistBySymbolAsync(attributeUnit.Symbol))
                return Result.Failure(AttributeUnitErrors.AttributeUnitAlreadyExistBySymbol(attributeUnit.Symbol));

            await _dbContext.AttributeUnits.AddAsync(attributeUnit);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(AttributeUnitsCacheKey);
            return Result.Success();
        }

        public async Task<Result<AttributeUnit>> GetAttributeUnitByIdAsync(int id)
        {
            var cacheKey = $"{AttributeUnitCacheKeyPrefix}{id}";
            var attributeUnit = await _cacheProvider.GetAsync<AttributeUnit>(cacheKey);
            if (attributeUnit == null)
            {
                attributeUnit = await _dbContext.AttributeUnits.AsNoTracking().FirstOrDefaultAsync(au => au.Id == id);
                if (attributeUnit == null)
                    return Result<AttributeUnit>.Failure(AttributeUnitErrors.AttributeUnitNotFoundById(id));

                await _cacheProvider.SetAsync(cacheKey, attributeUnit, TimeSpan.FromHours(1));
            }
            return Result<AttributeUnit>.Success(attributeUnit);
        }

        public async Task<Result<AttributeUnit>> GetAttributeUnitByNameAsync(string name)
        {
            var attributeUnit = await _dbContext.AttributeUnits.AsNoTracking().FirstOrDefaultAsync(au => au.Name == name);
            if (attributeUnit == null)
                return Result<AttributeUnit>.Failure(AttributeUnitErrors.AttributeUnitNotFoundByName(name));

            return Result<AttributeUnit>.Success(attributeUnit);
        }

        public async Task<Result<AttributeUnit>> GetAttributeUnitBySymbolAsync(string symbol)
        {
            var attributeUnit = await _dbContext.AttributeUnits.AsNoTracking().FirstOrDefaultAsync(au => au.Symbol == symbol);
            if (attributeUnit == null)
                return Result<AttributeUnit>.Failure(AttributeUnitErrors.AttributeUnitNotFoundBySymbol(symbol));

            return Result<AttributeUnit>.Success(attributeUnit);
        }

        public async Task<Result> UpdateAttributeUnitAsync(AttributeUnit attributeUnit)
        {
            var existingAttributeUnit = await _dbContext.AttributeUnits.FindAsync(attributeUnit.Id);
            if (existingAttributeUnit == null)
                return Result.Failure(AttributeUnitErrors.AttributeUnitNotFoundById(attributeUnit.Id));

            if (existingAttributeUnit.Name != attributeUnit.Name && await IsAttributeUnitExistByNameAsync(attributeUnit.Name))
                return Result.Failure(AttributeUnitErrors.AttributeUnitAlreadyExistByName(attributeUnit.Name));

            if (existingAttributeUnit.Symbol != attributeUnit.Symbol && await IsAttributeUnitExistBySymbolAsync(attributeUnit.Symbol))
                return Result.Failure(AttributeUnitErrors.AttributeUnitAlreadyExistBySymbol(attributeUnit.Symbol));

            _dbContext.Entry(existingAttributeUnit).CurrentValues.SetValues(attributeUnit);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(AttributeUnitsCacheKey);
            await _cacheProvider.RemoveAsync($"{AttributeUnitCacheKeyPrefix}{attributeUnit.Id}");
            return Result.Success();
        }

        public async Task<Result> DeleteAttributeUnitAsync(int id)
        {
            var attributeUnit = await _dbContext.AttributeUnits.FindAsync(id);
            if (attributeUnit == null)
                return Result.Failure(AttributeUnitErrors.AttributeUnitNotFoundById(id));

            _dbContext.AttributeUnits.Remove(attributeUnit);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(AttributeUnitsCacheKey);
            await _cacheProvider.RemoveAsync($"{AttributeUnitCacheKeyPrefix}{id}");
            return Result.Success();
        }

        private async Task<bool> IsAttributeUnitExistByNameAsync(string name)
        {
            return await _dbContext.AttributeUnits.AnyAsync(au => au.Name == name);
        }

        private async Task<bool> IsAttributeUnitExistBySymbolAsync(string symbol)
        {
            return await _dbContext.AttributeUnits.AnyAsync(au => au.Symbol == symbol);
        }
    }
}
