using Application.Abstraction.Cache;
using Domain.Abstraction;
using Domain.Entities;
using Domain.Errors.Audit;
using Domain.Repositories.Audit;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Audit
{
    public class EmployeeAuditRepository : IEmployeeAuditRepository
    {
        private readonly SiceDbContext _dbContext;
        private readonly ICacheProvider _cacheProvider;
        private const string EmployeeAuditsCacheKey = "employeeAuditsCache";
        private const string EmployeeAuditCacheKeyPrefix = "employeeAuditCache_";

        public EmployeeAuditRepository(SiceDbContext dbContext, ICacheProvider cacheProvider)
        {
            _dbContext = dbContext;
            _cacheProvider = cacheProvider;
        }

        public async Task<Result<IEnumerable<EmployeeAudit>>> GetAllEmployeeAuditsAsync()
        {
            var employeeAudits = await _cacheProvider.GetAsync<IEnumerable<EmployeeAudit>>(EmployeeAuditsCacheKey);
            if (employeeAudits == null)
            {
                employeeAudits = await _dbContext.EmployeeAudits.AsNoTracking().ToListAsync();
                await _cacheProvider.SetAsync(EmployeeAuditsCacheKey, employeeAudits, TimeSpan.FromHours(1));
            }
            return Result<IEnumerable<EmployeeAudit>>.Success(employeeAudits);
        }

        public async Task<Result> AddEmployeeAuditAsync(EmployeeAudit employeeAudit)
        {
            await _dbContext.EmployeeAudits.AddAsync(employeeAudit);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(EmployeeAuditsCacheKey);
            return Result.Success();
        }

        public async Task<Result<EmployeeAudit>> GetEmployeeAuditByIdAsync(int id)
        {
            var cacheKey = $"{EmployeeAuditCacheKeyPrefix}{id}";
            var employeeAudit = await _cacheProvider.GetAsync<EmployeeAudit>(cacheKey);
            if (employeeAudit == null)
            {
                employeeAudit = await _dbContext.EmployeeAudits.AsNoTracking().FirstOrDefaultAsync(ea => ea.Id == id);
                if (employeeAudit == null)
                    return Result<EmployeeAudit>.Failure(EmployeeAuditErrors.EmployeeAuditNotFoundById(id));

                await _cacheProvider.SetAsync(cacheKey, employeeAudit, TimeSpan.FromHours(1));
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
            await _cacheProvider.RemoveAsync(EmployeeAuditsCacheKey);
            await _cacheProvider.RemoveAsync($"{EmployeeAuditCacheKeyPrefix}{employeeAudit.Id}");
            return Result.Success();
        }

        public async Task<Result> DeleteEmployeeAuditAsync(int id)
        {
            var employeeAudit = await _dbContext.EmployeeAudits.FindAsync(id);
            if (employeeAudit == null)
                return Result.Failure(EmployeeAuditErrors.EmployeeAuditNotFoundById(id));

            _dbContext.EmployeeAudits.Remove(employeeAudit);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(EmployeeAuditsCacheKey);
            await _cacheProvider.RemoveAsync($"{EmployeeAuditCacheKeyPrefix}{id}");
            return Result.Success();
        }
    }
}
