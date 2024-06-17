using Domain.Abstraction;
using Domain.Entities;
using Domain.Errors.Staff;
using Domain.Repositories.Staff;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Repositories.Staff
{
    public class PositionRepository : IPositionRepository
    {
        private readonly SiceDbContext _dbContext;
        private readonly IMemoryCache _cache;
        private const string PositionsCacheKey = "positionsCache";
        private const string PositionCacheKeyPrefix = "positionCache_";

        public PositionRepository(SiceDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<Result<IEnumerable<Position>>> GetAllPositionsAsync()
        {
            if (!_cache.TryGetValue(PositionsCacheKey, out IEnumerable<Position> positions))
            {
                positions = await _dbContext.Positions.AsNoTracking().ToListAsync();
                _cache.Set(PositionsCacheKey, positions, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1)));
            }
            return Result<IEnumerable<Position>>.Success(positions);
        }

        public async Task<Result> AddPositionAsync(Position position)
        {
            if (await IsPositionExistByNameAsync(position.Name))
                return Result.Failure(PositionErrors.PositionAlreadyExist(position.Name));

            await _dbContext.Positions.AddAsync(position);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(PositionsCacheKey);
            return Result.Success();
        }

        public async Task<Result<Position>> GetPositionByIdAsync(int id)
        {
            var cacheKey = $"{PositionCacheKeyPrefix}{id}";
            if (!_cache.TryGetValue(cacheKey, out Position position))
            {
                position = await _dbContext.Positions.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                if (position == null)
                    return Result<Position>.Failure(PositionErrors.PositionNotFoundById(id));

                _cache.Set(cacheKey, position, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1)));
            }
            return Result<Position>.Success(position);
        }

        public async Task<Result<Position>> GetPositionByNameAsync(string name)
        {
            var position = await _dbContext.Positions.AsNoTracking().FirstOrDefaultAsync(p => p.Name == name);
            if (position == null)
                return Result<Position>.Failure(PositionErrors.PositionNotFoundByName(name));

            return Result<Position>.Success(position);
        }

        public async Task<Result> UpdatePositionAsync(Position position)
        {
            var existingPosition = await _dbContext.Positions.FindAsync(position.Id);
            if (existingPosition == null)
                return Result.Failure(PositionErrors.PositionNotFoundById(position.Id));

            if (existingPosition.Name != position.Name && await IsPositionExistByNameAsync(position.Name))
                return Result.Failure(PositionErrors.PositionAlreadyExist(position.Name));

            _dbContext.Entry(existingPosition).CurrentValues.SetValues(position);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(PositionsCacheKey);
            _cache.Remove($"{PositionCacheKeyPrefix}{position.Id}");
            return Result.Success();
        }

        public async Task<Result> DeletePositionAsync(int id)
        {
            var position = await _dbContext.Positions.FindAsync(id);
            if (position == null)
                return Result.Failure(PositionErrors.PositionNotFoundById(id));

            _dbContext.Positions.Remove(position);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(PositionsCacheKey);
            _cache.Remove($"{PositionCacheKeyPrefix}{id}");
            return Result.Success();
        }

        private async Task<bool> IsPositionExistByNameAsync(string name)
        {
            return await _dbContext.Positions.AnyAsync(p => p.Name == name);
        }
    }
}
