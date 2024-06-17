using Domain.Abstraction;
using Domain.Entities;
using Domain.Errors.Audit;
using Domain.Repositories.Audit;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Repositories.Audit
{
    public class UserAuditRepository : IUserAuditRepository
    {
        private readonly SiceDbContext _dbContext;
        private readonly IMemoryCache _cache;
        private const string UserAuditsCacheKey = "userAuditsCache";
        private const string UserAuditCacheKeyPrefix = "userAuditCache_";

        public UserAuditRepository(SiceDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<Result<IEnumerable<UserInfoAudit>>> GetAllUserAuditsAsync()
        {
            if (!_cache.TryGetValue(UserAuditsCacheKey, out IEnumerable<UserInfoAudit> userAudits))
            {
                userAudits = await _dbContext.UserInfoAudits.AsNoTracking().ToListAsync();
                _cache.Set(UserAuditsCacheKey, userAudits, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1)));
            }
            return Result<IEnumerable<UserInfoAudit>>.Success(userAudits);
        }

        public async Task<Result> AddUserAuditAsync(UserInfoAudit userAudit)
        {
            await _dbContext.UserInfoAudits.AddAsync(userAudit);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(UserAuditsCacheKey);
            return Result.Success();
        }

        public async Task<Result<UserInfoAudit>> GetUserAuditByIdAsync(int id)
        {
            var cacheKey = $"{UserAuditCacheKeyPrefix}{id}";
            if (!_cache.TryGetValue(cacheKey, out UserInfoAudit userAudit))
            {
                userAudit = await _dbContext.UserInfoAudits.AsNoTracking().FirstOrDefaultAsync(ua => ua.Id == id);
                if (userAudit == null)
                    return Result<UserInfoAudit>.Failure(UserAuditErrors.UserAuditNotFoundById(id));

                _cache.Set(cacheKey, userAudit, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1)));
            }
            return Result<UserInfoAudit>.Success(userAudit);
        }

        public async Task<Result<IEnumerable<UserInfoAudit>>> GetUserAuditsByUserIdAsync(int userId)
        {
            var userAudits = await _dbContext.UserInfoAudits.AsNoTracking().Where(ua => ua.UserInfoId == userId).ToListAsync();
            return Result<IEnumerable<UserInfoAudit>>.Success(userAudits);
        }

        public async Task<Result<IEnumerable<UserInfoAudit>>> GetUserAuditsByEmployeeIdAsync(int employeeId)
        {
            var userAudits = await _dbContext.UserInfoAudits.AsNoTracking().Where(ua => ua.EmployeeId == employeeId).ToListAsync();
            return Result<IEnumerable<UserInfoAudit>>.Success(userAudits);
        }

        public async Task<Result> UpdateUserAuditAsync(UserInfoAudit userAudit)
        {
            var existingAudit = await _dbContext.UserInfoAudits.FindAsync(userAudit.Id);
            if (existingAudit == null)
                return Result.Failure(UserAuditErrors.UserAuditNotFoundById(userAudit.Id));

            _dbContext.Entry(existingAudit).CurrentValues.SetValues(userAudit);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(UserAuditsCacheKey);
            _cache.Remove($"{UserAuditCacheKeyPrefix}{userAudit.Id}");
            return Result.Success();
        }

        public async Task<Result> DeleteUserAuditAsync(int id)
        {
            var userAudit = await _dbContext.UserInfoAudits.FindAsync(id);
            if (userAudit == null)
                return Result.Failure(UserAuditErrors.UserAuditNotFoundById(id));

            _dbContext.UserInfoAudits.Remove(userAudit);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(UserAuditsCacheKey);
            _cache.Remove($"{UserAuditCacheKeyPrefix}{id}");
            return Result.Success();
        }
    }
}
