using Domain.Abstraction;
using Domain.Entities;
using Domain.Errors.Addresses;
using Domain.Repositories.Addresses;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Repositories.Addresses
{
    public class RegionRepository : IRegionRepository
    {
        private readonly SiceDbContext _dbContext;
        private readonly IMemoryCache _cache;
        private const string RegionsCacheKey = "regionsCache";
        private const string RegionCacheKeyPrefix = "regionCache_";

        public RegionRepository(SiceDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<Result<IEnumerable<Region>>> GetAllRegionsAsync()
        {
            if (!_cache.TryGetValue(RegionsCacheKey, out IEnumerable<Region> regions))
            {
                regions = await _dbContext.Regions.AsNoTracking().ToListAsync();
                _cache.Set(RegionsCacheKey, regions, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1)));
            }
            return Result<IEnumerable<Region>>.Success(regions);
        }

        public async Task<Result> AddRegionAsync(Region region)
        {
            if (await IsRegionExistByNameAsync(region.Name))
                return Result.Failure(RegionErrors.RegionAlreadyExist(region.Name));

            await _dbContext.Regions.AddAsync(region);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(RegionsCacheKey);
            return Result.Success();
        }

        public async Task<Result<Region>> GetRegionByIdAsync(int id)
        {
            var cacheKey = $"{RegionCacheKeyPrefix}{id}";
            if (!_cache.TryGetValue(cacheKey, out Region region))
            {
                region = await _dbContext.Regions.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
                if (region == null)
                    return Result<Region>.Failure(RegionErrors.RegionNotFoundById(id));

                _cache.Set(cacheKey, region, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1)));
            }
            return Result<Region>.Success(region);
        }

        public async Task<Result<Region>> GetRegionByNameAsync(string name)
        {
            var region = await _dbContext.Regions.AsNoTracking().FirstOrDefaultAsync(r => r.Name == name);
            if (region == null)
                return Result<Region>.Failure(RegionErrors.RegionNotFoundByName(name));

            return Result<Region>.Success(region);
        }

        public async Task<Result> UpdateRegionAsync(Region region)
        {
            var existingRegion = await _dbContext.Regions.FindAsync(region.Id);
            if (existingRegion == null)
                return Result.Failure(RegionErrors.RegionNotFoundById(region.Id));

            if (existingRegion.Name != region.Name && await IsRegionExistByNameAsync(region.Name))
                return Result.Failure(RegionErrors.RegionAlreadyExist(region.Name));

            _dbContext.Entry(existingRegion).CurrentValues.SetValues(region);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(RegionsCacheKey);
            _cache.Remove($"{RegionCacheKeyPrefix}{region.Id}");
            return Result.Success();
        }

        public async Task<Result> DeleteRegionAsync(int id)
        {
            var region = await _dbContext.Regions.FindAsync(id);
            if (region == null)
                return Result.Failure(RegionErrors.RegionNotFoundById(id));

            _dbContext.Regions.Remove(region);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(RegionsCacheKey);
            _cache.Remove($"{RegionCacheKeyPrefix}{id}");
            return Result.Success();
        }

        private async Task<bool> IsRegionExistByNameAsync(string name)
        {
            return await _dbContext.Regions.AnyAsync(r => r.Name == name);
        }
    }
}
