using Application.Abstraction.Cache;
using Domain.Abstraction;
using Domain.Entities;
using Domain.Errors.Items.Attributes;
using Domain.Repositories.Item.Attributes;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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
            var cachedAttributeUnits = await _cacheProvider.GetAsync<IEnumerable<AttributeUnit>>(AttributeUnitsCacheKey);
            if (cachedAttributeUnits != null)
                return Result<IEnumerable<AttributeUnit>>.Success(cachedAttributeUnits);

            var attributeUnits = await _dbContext.AttributeUnits.AsNoTracking().ToListAsync();
            await _cacheProvider.SetAsync(AttributeUnitsCacheKey, attributeUnits);
            return Result<IEnumerable<AttributeUnit>>.Success(attributeUnits);
        }

        public async Task<Result> AddAttributeUnitAsync(AttributeUnit attributeUnit)
        {
            await _dbContext.AttributeUnits.AddAsync(attributeUnit);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(AttributeUnitsCacheKey);
            return Result.Success();
        }

        public async Task<Result<AttributeUnit>> GetAttributeUnitByIdAsync(int id)
        {
            var cacheKey = $"{AttributeUnitCacheKeyPrefix}{id}";
            var cachedAttributeUnit = await _cacheProvider.GetAsync<AttributeUnit>(cacheKey);
            if (cachedAttributeUnit != null)
                return Result<AttributeUnit>.Success(cachedAttributeUnit);

            var attributeUnit = await _dbContext.AttributeUnits.FindAsync(id);
            if (attributeUnit != null)
            {
                await _cacheProvider.SetAsync(cacheKey, attributeUnit);
                return Result<AttributeUnit>.Success(attributeUnit);
            }
            return Result<AttributeUnit>.Failure(AttributeUnitErrors.AttributeUnitNotFoundById(id));
        }

        public async Task<Result<AttributeUnit>> GetAttributeUnitByNameAsync(string name)
        {
            var attributeUnit = await _dbContext.AttributeUnits.FirstOrDefaultAsync(au => au.Name == name);
            return attributeUnit != null
                ? Result<AttributeUnit>.Success(attributeUnit)
                : Result<AttributeUnit>.Failure(AttributeUnitErrors.AttributeUnitNotFoundByName(name));
        }

        public async Task<Result<AttributeUnit>> GetAttributeUnitBySymbolAsync(string symbol)
        {
            var attributeUnit = await _dbContext.AttributeUnits.FirstOrDefaultAsync(au => au.Symbol == symbol);
            return attributeUnit != null
                ? Result<AttributeUnit>.Success(attributeUnit)
                : Result<AttributeUnit>.Failure(AttributeUnitErrors.AttributeUnitNotFoundBySymbol(symbol));
        }

        public async Task<Result> UpdateAttributeUnitAsync(AttributeUnit attributeUnit)
        {
            var existingAttributeUnit = await _dbContext.AttributeUnits.FindAsync(attributeUnit.Id);
            if (existingAttributeUnit == null)
                return Result.Failure(AttributeUnitErrors.AttributeUnitNotFoundById(attributeUnit.Id));

            _dbContext.Entry(existingAttributeUnit).CurrentValues.SetValues(attributeUnit);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync($"{AttributeUnitCacheKeyPrefix}{attributeUnit.Id}");
            await _cacheProvider.RemoveAsync(AttributeUnitsCacheKey);
            return Result.Success();
        }

        public async Task<Result> DeleteAttributeUnitAsync(int id)
        {
            var attributeUnit = await _dbContext.AttributeUnits.FindAsync(id);
            if (attributeUnit == null)
                return Result.Failure(AttributeUnitErrors.AttributeUnitNotFoundById(id));

            _dbContext.AttributeUnits.Remove(attributeUnit);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync($"{AttributeUnitCacheKeyPrefix}{id}");
            await _cacheProvider.RemoveAsync(AttributeUnitsCacheKey);
            return Result.Success();
        }

        // Eager loading methods
        public async Task<Result<IEnumerable<AttributeUnit>>> GetAllAttributeUnitsWithEntitiesAsync()
        {
            var attributeUnits = await _dbContext.AttributeUnits
                .Include(au => au.UnitCategory)
                .Include(au => au.AttributeValueType)
                .AsNoTracking()
                .ToListAsync();
            return Result<IEnumerable<AttributeUnit>>.Success(attributeUnits);
        }

        public async Task<Result<AttributeUnit>> GetAttributeUnitWithEntitiesByIdAsync(int id)
        {
            var attributeUnit = await _dbContext.AttributeUnits
                .Include(au => au.UnitCategory)
                .Include(au => au.AttributeValueType)
                .FirstOrDefaultAsync(au => au.Id == id);
            return attributeUnit != null
                ? Result<AttributeUnit>.Success(attributeUnit)
                : Result<AttributeUnit>.Failure(AttributeUnitErrors.AttributeUnitNotFoundById(id));
        }
    }
}