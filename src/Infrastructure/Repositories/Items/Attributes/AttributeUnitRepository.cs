using Domain.Abstraction;
using Domain.Entities;
using Domain.Errors.Items.Attributes;
using Domain.Repositories.Item.Attributes;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Repositories.Items.Attributes
{
    public class AttributeUnitRepository : IAttributeUnitRepository
    {
        private readonly SiceDbContext _dbContext;
        private readonly IMemoryCache _cache;
        private const string AttributeUnitsCacheKey = "attributeUnitsCache";
        private const string AttributeUnitCacheKeyPrefix = "attributeUnitCache_";

        public AttributeUnitRepository(SiceDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<Result<IEnumerable<AttributeUnit>>> GetAllAttributeUnitsAsync()
        {
            if (!_cache.TryGetValue(AttributeUnitsCacheKey, out IEnumerable<AttributeUnit> attributeUnits))
            {
                attributeUnits = await _dbContext.AttributeUnits.AsNoTracking().ToListAsync();
                _cache.Set(AttributeUnitsCacheKey, attributeUnits, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1)));
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
            _cache.Remove(AttributeUnitsCacheKey);
            return Result.Success();
        }

        public async Task<Result<AttributeUnit>> GetAttributeUnitByIdAsync(int id)
        {
            var cacheKey = $"{AttributeUnitCacheKeyPrefix}{id}";
            if (!_cache.TryGetValue(cacheKey, out AttributeUnit attributeUnit))
            {
                attributeUnit = await _dbContext.AttributeUnits.AsNoTracking().FirstOrDefaultAsync(au => au.Id == id);
                if (attributeUnit == null)
                    return Result<AttributeUnit>.Failure(AttributeUnitErrors.AttributeUnitNotFoundById(id));

                _cache.Set(cacheKey, attributeUnit, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1)));
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
            _cache.Remove(AttributeUnitsCacheKey);
            _cache.Remove($"{AttributeUnitCacheKeyPrefix}{attributeUnit.Id}");
            return Result.Success();
        }

        public async Task<Result> DeleteAttributeUnitAsync(int id)
        {
            var attributeUnit = await _dbContext.AttributeUnits.FindAsync(id);
            if (attributeUnit == null)
                return Result.Failure(AttributeUnitErrors.AttributeUnitNotFoundById(id));

            _dbContext.AttributeUnits.Remove(attributeUnit);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(AttributeUnitsCacheKey);
            _cache.Remove($"{AttributeUnitCacheKeyPrefix}{id}");
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
