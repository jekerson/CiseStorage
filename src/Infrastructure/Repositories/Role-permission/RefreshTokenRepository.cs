using Application.Abstraction.Cache;
using Domain.Abstraction;
using Domain.Entities;
using Domain.Errors;
using Domain.Repositories.Role_permission;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Role_permission
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly SiceDbContext _dbContext;
        private readonly ICacheProvider _cacheProvider;
        private const string RefreshTokensCacheKey = "refreshTokensCache";
        private const string RefreshTokenCacheKeyPrefix = "refreshTokenCache_";

        public RefreshTokenRepository(SiceDbContext dbContext, ICacheProvider cacheProvider)
        {
            _dbContext = dbContext;
            _cacheProvider = cacheProvider;
        }

        public async Task<Result<IEnumerable<RefreshToken>>> GetAllRefreshTokensAsync()
        {
            var refreshTokens = await _cacheProvider.GetAsync<IEnumerable<RefreshToken>>(RefreshTokensCacheKey);
            if (refreshTokens == null)
            {
                refreshTokens = await _dbContext.RefreshTokens.AsNoTracking().ToListAsync();
                await _cacheProvider.SetAsync(RefreshTokensCacheKey, refreshTokens, TimeSpan.FromHours(1));
            }
            return Result<IEnumerable<RefreshToken>>.Success(refreshTokens);
        }

        public async Task<Result> AddRefreshTokenAsync(RefreshToken refreshToken)
        {
            if (await IsTokenExistAsync(refreshToken.Token))
                return Result.Failure(RefreshTokenErrors.TokenAlreadyExist(refreshToken.Token));

            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(RefreshTokensCacheKey);
            return Result.Success();
        }

        public async Task<Result<RefreshToken>> GetRefreshTokenByIdAsync(int id)
        {
            var cacheKey = $"{RefreshTokenCacheKeyPrefix}{id}";
            var refreshToken = await _cacheProvider.GetAsync<RefreshToken>(cacheKey);
            if (refreshToken == null)
            {
                refreshToken = await _dbContext.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
                if (refreshToken == null)
                    return Result<RefreshToken>.Failure(RefreshTokenErrors.TokenNotFoundById(id));

                await _cacheProvider.SetAsync(cacheKey, refreshToken, TimeSpan.FromHours(1));
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
            await _cacheProvider.RemoveAsync(RefreshTokensCacheKey);
            await _cacheProvider.RemoveAsync($"{RefreshTokenCacheKeyPrefix}{refreshToken.Id}");
            return Result.Success();
        }

        public async Task<Result> DeleteRefreshTokenAsync(int id)
        {
            var refreshToken = await _dbContext.RefreshTokens.FindAsync(id);
            if (refreshToken == null)
                return Result.Failure(RefreshTokenErrors.TokenNotFoundById(id));

            _dbContext.RefreshTokens.Remove(refreshToken);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(RefreshTokensCacheKey);
            await _cacheProvider.RemoveAsync($"{RefreshTokenCacheKeyPrefix}{id}");
            return Result.Success();
        }

        private async Task<bool> IsTokenExistAsync(string token)
        {
            return await _dbContext.RefreshTokens.AnyAsync(t => t.Token == token);
        }
    }
}
