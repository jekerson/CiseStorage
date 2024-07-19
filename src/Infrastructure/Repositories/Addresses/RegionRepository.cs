using Application.Abstraction.Cache;
using Domain.Abstraction;
using Domain.Entities;
using Domain.Errors.Addresses;
using Domain.Repositories.Addresses;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Addresses
{
    public class RegionRepository : IRegionRepository
    {
        private readonly SiceDbContext _dbContext;
        private readonly ICacheProvider _cacheProvider;
        private const string RegionsCacheKey = "regionsCache";
        private const string RegionCacheKeyPrefix = "regionCache_";

        public RegionRepository(SiceDbContext dbContext, ICacheProvider cacheProvider)
        {
            _dbContext = dbContext;
            _cacheProvider = cacheProvider;
        }

        public async Task<Result<IEnumerable<Region>>> GetAllRegionsAsync()
        {
            var regions = await _cacheProvider.GetAsync<IEnumerable<Region>>(RegionsCacheKey);
            if (regions == null)
            {
                regions = await _dbContext.Regions.AsNoTracking().ToListAsync();
                await _cacheProvider.SetAsync(RegionsCacheKey, regions, TimeSpan.FromHours(1));
            }
            return Result<IEnumerable<Region>>.Success(regions);
        }

        public async Task<Result> AddRegionAsync(Region region)
        {
            if (await IsRegionExistByNameAsync(region.Name))
                return Result.Failure(RegionErrors.RegionAlreadyExist(region.Name));

            await _dbContext.Regions.AddAsync(region);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(RegionsCacheKey);
            return Result.Success();
        }

        public async Task<Result<Region>> GetRegionByIdAsync(int id)
        {
            var cacheKey = $"{RegionCacheKeyPrefix}{id}";
            var region = await _cacheProvider.GetAsync<Region>(cacheKey);
            if (region == null)
            {
                region = await _dbContext.Regions.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
                if (region == null)
                    return Result<Region>.Failure(RegionErrors.RegionNotFoundById(id));

                await _cacheProvider.SetAsync(cacheKey, region, TimeSpan.FromHours(1));
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
            await _cacheProvider.RemoveAsync(RegionsCacheKey);
            await _cacheProvider.RemoveAsync($"{RegionCacheKeyPrefix}{region.Id}");
            return Result.Success();
        }

        public async Task<Result> DeleteRegionAsync(int id)
        {
            var region = await _dbContext.Regions.FindAsync(id);
            if (region == null)
                return Result.Failure(RegionErrors.RegionNotFoundById(id));

            _dbContext.Regions.Remove(region);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(RegionsCacheKey);
            await _cacheProvider.RemoveAsync($"{RegionCacheKeyPrefix}{id}");
            return Result.Success();
        }

        private async Task<bool> IsRegionExistByNameAsync(string name)
        {
            return await _dbContext.Regions.AnyAsync(r => r.Name == name);
        }
    }
}
