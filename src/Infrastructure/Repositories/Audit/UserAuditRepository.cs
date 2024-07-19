using Application.Abstraction.Cache;
using Domain.Abstraction;
using Domain.Entities;
using Domain.Errors.Audit;
using Domain.Repositories.Audit;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Audit
{
    public class UserAuditRepository : IUserAuditRepository
    {
        private readonly SiceDbContext _dbContext;
        private readonly ICacheProvider _cacheProvider;
        private const string UserAuditsCacheKey = "userAuditsCache";
        private const string UserAuditCacheKeyPrefix = "userAuditCache_";

        public UserAuditRepository(SiceDbContext dbContext, ICacheProvider cacheProvider)
        {
            _dbContext = dbContext;
            _cacheProvider = cacheProvider;
        }

        public async Task<Result<IEnumerable<UserInfoAudit>>> GetAllUserAuditsAsync()
        {
            var userAudits = await _cacheProvider.GetAsync<IEnumerable<UserInfoAudit>>(UserAuditsCacheKey);
            if (userAudits == null)
            {
                userAudits = await _dbContext.UserInfoAudits.AsNoTracking().ToListAsync();
                await _cacheProvider.SetAsync(UserAuditsCacheKey, userAudits, TimeSpan.FromHours(1));
            }
            return Result<IEnumerable<UserInfoAudit>>.Success(userAudits);
        }

        public async Task<Result> AddUserAuditAsync(UserInfoAudit userAudit)
        {
            await _dbContext.UserInfoAudits.AddAsync(userAudit);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(UserAuditsCacheKey);
            return Result.Success();
        }

        public async Task<Result<UserInfoAudit>> GetUserAuditByIdAsync(int id)
        {
            var cacheKey = $"{UserAuditCacheKeyPrefix}{id}";
            var userAudit = await _cacheProvider.GetAsync<UserInfoAudit>(cacheKey);
            if (userAudit == null)
            {
                userAudit = await _dbContext.UserInfoAudits.AsNoTracking().FirstOrDefaultAsync(ua => ua.Id == id);
                if (userAudit == null)
                    return Result<UserInfoAudit>.Failure(UserAuditErrors.UserAuditNotFoundById(id));

                await _cacheProvider.SetAsync(cacheKey, userAudit, TimeSpan.FromHours(1));
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
            await _cacheProvider.RemoveAsync(UserAuditsCacheKey);
            await _cacheProvider.RemoveAsync($"{UserAuditCacheKeyPrefix}{userAudit.Id}");
            return Result.Success();
        }

        public async Task<Result> DeleteUserAuditAsync(int id)
        {
            var userAudit = await _dbContext.UserInfoAudits.FindAsync(id);
            if (userAudit == null)
                return Result.Failure(UserAuditErrors.UserAuditNotFoundById(id));

            _dbContext.UserInfoAudits.Remove(userAudit);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(UserAuditsCacheKey);
            await _cacheProvider.RemoveAsync($"{UserAuditCacheKeyPrefix}{id}");
            return Result.Success();
        }
    }
}
