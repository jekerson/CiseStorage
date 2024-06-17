using Domain.Abstraction;
using Domain.Entities;
using Domain.Errors;
using Domain.Repositories.Role_permission;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Repositories.Role_permission
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly SiceDbContext _dbContext;
        private readonly IMemoryCache _cache;
        private const string RefreshTokensCacheKey = "refreshTokensCache";
        private const string RefreshTokenCacheKeyPrefix = "refreshTokenCache_";

        public RefreshTokenRepository(SiceDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<Result<IEnumerable<RefreshToken>>> GetAllRefreshTokensAsync()
        {
            if (!_cache.TryGetValue(RefreshTokensCacheKey, out IEnumerable<RefreshToken> refreshTokens))
            {
                refreshTokens = await _dbContext.RefreshTokens.AsNoTracking().ToListAsync();
                _cache.Set(RefreshTokensCacheKey, refreshTokens, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1)));
            }
            return Result<IEnumerable<RefreshToken>>.Success(refreshTokens);
        }

        public async Task<Result> AddRefreshTokenAsync(RefreshToken refreshToken)
        {
            if (await IsTokenExistAsync(refreshToken.Token))
                return Result.Failure(RefreshTokenErrors.TokenAlreadyExist(refreshToken.Token));

            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(RefreshTokensCacheKey);
            return Result.Success();
        }

        public async Task<Result<RefreshToken>> GetRefreshTokenByIdAsync(int id)
        {
            var cacheKey = $"{RefreshTokenCacheKeyPrefix}{id}";
            if (!_cache.TryGetValue(cacheKey, out RefreshToken refreshToken))
            {
                refreshToken = await _dbContext.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
                if (refreshToken == null)
                    return Result<RefreshToken>.Failure(RefreshTokenErrors.TokenNotFoundById(id));

                _cache.Set(cacheKey, refreshToken, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1)));
            }
            return Result<RefreshToken>.Success(refreshToken);
        }

        public async Task<Result<RefreshToken>> GetRefreshTokenByTokenAsync(string token)
        {
            var refreshToken = await _dbContext.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(t => t.Token == token);
            if (refreshToken == null)
                return Result<RefreshToken>.Failure(RefreshTokenErrors.TokenNotFoundByToken(token));

            return Result<RefreshToken>.Success(refreshToken);
        }

        public async Task<Result> UpdateRefreshTokenAsync(RefreshToken refreshToken)
        {
            var existingToken = await _dbContext.RefreshTokens.FindAsync(refreshToken.Id);
            if (existingToken == null)
                return Result.Failure(RefreshTokenErrors.TokenNotFoundById(refreshToken.Id));

            if (existingToken.Token != refreshToken.Token && await IsTokenExistAsync(refreshToken.Token))
                return Result.Failure(RefreshTokenErrors.TokenAlreadyExist(refreshToken.Token));

            _dbContext.Entry(existingToken).CurrentValues.SetValues(refreshToken);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(RefreshTokensCacheKey);
            _cache.Remove($"{RefreshTokenCacheKeyPrefix}{refreshToken.Id}");
            return Result.Success();
        }

        public async Task<Result> DeleteRefreshTokenAsync(int id)
        {
            var refreshToken = await _dbContext.RefreshTokens.FindAsync(id);
            if (refreshToken == null)
                return Result.Failure(RefreshTokenErrors.TokenNotFoundById(id));

            _dbContext.RefreshTokens.Remove(refreshToken);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(RefreshTokensCacheKey);
            _cache.Remove($"{RefreshTokenCacheKeyPrefix}{id}");
            return Result.Success();
        }

        private async Task<bool> IsTokenExistAsync(string token)
        {
            return await _dbContext.RefreshTokens.AnyAsync(t => t.Token == token);
        }
    }
}
