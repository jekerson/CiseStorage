using Domain.Abstraction;
using Domain.Entities;
using Domain.Errors.Audit;
using Domain.Repositories.Audit;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Repositories.Audit
{
    public class ItemResponsibilityAuditRepository : IItemResponsibilityAuditRepository
    {
        private readonly SiceDbContext _dbContext;
        private readonly IMemoryCache _cache;
        private const string ItemResponsibilityAuditsCacheKey = "itemResponsibilityAuditsCache";
        private const string ItemResponsibilityAuditCacheKeyPrefix = "itemResponsibilityAuditCache_";

        public ItemResponsibilityAuditRepository(SiceDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<Result<IEnumerable<ItemResponsibilityAudit>>> GetAllItemResponsibilityAuditsAsync()
        {
            if (!_cache.TryGetValue(ItemResponsibilityAuditsCacheKey, out IEnumerable<ItemResponsibilityAudit> itemResponsibilityAudits))
            {
                itemResponsibilityAudits = await _dbContext.ItemResponsibilityAudits.AsNoTracking().ToListAsync();
                _cache.Set(ItemResponsibilityAuditsCacheKey, itemResponsibilityAudits, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1)));
            }
            return Result<IEnumerable<ItemResponsibilityAudit>>.Success(itemResponsibilityAudits);
        }

        public async Task<Result> AddItemResponsibilityAuditAsync(ItemResponsibilityAudit itemResponsibilityAudit)
        {
            await _dbContext.ItemResponsibilityAudits.AddAsync(itemResponsibilityAudit);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(ItemResponsibilityAuditsCacheKey);
            return Result.Success();
        }

        public async Task<Result<ItemResponsibilityAudit>> GetItemResponsibilityAuditByIdAsync(int id)
        {
            var cacheKey = $"{ItemResponsibilityAuditCacheKeyPrefix}{id}";
            if (!_cache.TryGetValue(cacheKey, out ItemResponsibilityAudit itemResponsibilityAudit))
            {
                itemResponsibilityAudit = await _dbContext.ItemResponsibilityAudits.AsNoTracking().FirstOrDefaultAsync(ira => ira.Id == id);
                if (itemResponsibilityAudit == null)
                    return Result<ItemResponsibilityAudit>.Failure(ItemResponsibilityAuditErrors.ItemResponsibilityAuditNotFoundById(id));

                _cache.Set(cacheKey, itemResponsibilityAudit, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1)));
            }
            return Result<ItemResponsibilityAudit>.Success(itemResponsibilityAudit);
        }

        public async Task<Result<IEnumerable<ItemResponsibilityAudit>>> GetItemResponsibilityAuditsByItemIdAsync(int itemId)
        {
            var itemResponsibilityAudits = await _dbContext.ItemResponsibilityAudits.AsNoTracking()
                .Where(ira => ira.ItemId == itemId)
                .ToListAsync();
            return Result<IEnumerable<ItemResponsibilityAudit>>.Success(itemResponsibilityAudits);
        }

        public async Task<Result<IEnumerable<ItemResponsibilityAudit>>> GetItemResponsibilityAuditsByChangedByAsync(int userInfoId)
        {
            var itemResponsibilityAudits = await _dbContext.ItemResponsibilityAudits.AsNoTracking()
                .Where(ira => ira.ChangedBy == userInfoId)
                .ToListAsync();
            return Result<IEnumerable<ItemResponsibilityAudit>>.Success(itemResponsibilityAudits);
        }

        public async Task<Result<IEnumerable<ItemResponsibilityAudit>>> GetItemResponsibilityAuditsByEmployeeIdAsync(int employeeId)
        {
            var itemResponsibilityAudits = await _dbContext.ItemResponsibilityAudits.AsNoTracking()
                .Where(ira => ira.EmployeeId == employeeId)
                .ToListAsync();
            return Result<IEnumerable<ItemResponsibilityAudit>>.Success(itemResponsibilityAudits);
        }

        public async Task<Result> UpdateItemResponsibilityAuditAsync(ItemResponsibilityAudit itemResponsibilityAudit)
        {
            var existingItemResponsibilityAudit = await _dbContext.ItemResponsibilityAudits.FindAsync(itemResponsibilityAudit.Id);
            if (existingItemResponsibilityAudit == null)
                return Result.Failure(ItemResponsibilityAuditErrors.ItemResponsibilityAuditNotFoundById(itemResponsibilityAudit.Id));

            _dbContext.Entry(existingItemResponsibilityAudit).CurrentValues.SetValues(itemResponsibilityAudit);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(ItemResponsibilityAuditsCacheKey);
            _cache.Remove($"{ItemResponsibilityAuditCacheKeyPrefix}{itemResponsibilityAudit.Id}");
            return Result.Success();
        }

        public async Task<Result> DeleteItemResponsibilityAuditAsync(int id)
        {
            var itemResponsibilityAudit = await _dbContext.ItemResponsibilityAudits.FindAsync(id);
            if (itemResponsibilityAudit == null)
                return Result.Failure(ItemResponsibilityAuditErrors.ItemResponsibilityAuditNotFoundById(id));

            _dbContext.ItemResponsibilityAudits.Remove(itemResponsibilityAudit);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(ItemResponsibilityAuditsCacheKey);
            _cache.Remove($"{ItemResponsibilityAuditCacheKeyPrefix}{id}");
            return Result.Success();
        }
    }
}
