using Domain.Abstraction;
using Domain.Entities;
using Domain.Errors.Audit;
using Domain.Repositories.Audit;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Repositories.Audit
{
    public class EmployeeAuditRepository : IEmployeeAuditRepository
    {
        private readonly SiceDbContext _dbContext;
        private readonly IMemoryCache _cache;
        private const string EmployeeAuditsCacheKey = "employeeAuditsCache";
        private const string EmployeeAuditCacheKeyPrefix = "employeeAuditCache_";

        public EmployeeAuditRepository(SiceDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<Result<IEnumerable<EmployeeAudit>>> GetAllEmployeeAuditsAsync()
        {
            if (!_cache.TryGetValue(EmployeeAuditsCacheKey, out IEnumerable<EmployeeAudit> employeeAudits))
            {
                employeeAudits = await _dbContext.EmployeeAudits.AsNoTracking().ToListAsync();
                _cache.Set(EmployeeAuditsCacheKey, employeeAudits, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1)));
            }
            return Result<IEnumerable<EmployeeAudit>>.Success(employeeAudits);
        }

        public async Task<Result> AddEmployeeAuditAsync(EmployeeAudit employeeAudit)
        {
            await _dbContext.EmployeeAudits.AddAsync(employeeAudit);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(EmployeeAuditsCacheKey);
            return Result.Success();
        }

        public async Task<Result<EmployeeAudit>> GetEmployeeAuditByIdAsync(int id)
        {
            var cacheKey = $"{EmployeeAuditCacheKeyPrefix}{id}";
            if (!_cache.TryGetValue(cacheKey, out EmployeeAudit employeeAudit))
            {
                employeeAudit = await _dbContext.EmployeeAudits.AsNoTracking().FirstOrDefaultAsync(ea => ea.Id == id);
                if (employeeAudit == null)
                    return Result<EmployeeAudit>.Failure(EmployeeAuditErrors.EmployeeAuditNotFoundById(id));

                _cache.Set(cacheKey, employeeAudit, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1)));
            }
            return Result<EmployeeAudit>.Success(employeeAudit);
        }

        public async Task<Result<IEnumerable<EmployeeAudit>>> GetEmployeeAuditsByEmployeeIdAsync(int employeeId)
        {
            var employeeAudits = await _dbContext.EmployeeAudits.AsNoTracking().Where(ea => ea.EmployeeId == employeeId).ToListAsync();
            return Result<IEnumerable<EmployeeAudit>>.Success(employeeAudits);
        }

        public async Task<Result> UpdateEmployeeAuditAsync(EmployeeAudit employeeAudit)
        {
            var existingAudit = await _dbContext.EmployeeAudits.FindAsync(employeeAudit.Id);
            if (existingAudit == null)
                return Result.Failure(EmployeeAuditErrors.EmployeeAuditNotFoundById(employeeAudit.Id));

            _dbContext.Entry(existingAudit).CurrentValues.SetValues(employeeAudit);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(EmployeeAuditsCacheKey);
            _cache.Remove($"{EmployeeAuditCacheKeyPrefix}{employeeAudit.Id}");
            return Result.Success();
        }

        public async Task<Result> DeleteEmployeeAuditAsync(int id)
        {
            var employeeAudit = await _dbContext.EmployeeAudits.FindAsync(id);
            if (employeeAudit == null)
                return Result.Failure(EmployeeAuditErrors.EmployeeAuditNotFoundById(id));

            _dbContext.EmployeeAudits.Remove(employeeAudit);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(EmployeeAuditsCacheKey);
            _cache.Remove($"{EmployeeAuditCacheKeyPrefix}{id}");
            return Result.Success();
        }
    }
}
