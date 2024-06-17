using Domain.Abstraction;
using Domain.Entities;
using Domain.Errors.Audit;
using Domain.Repositories.Audit;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Repositories.Audit
{
    public class ItemAuditRepository : IItemAuditRepository
    {
        private readonly SiceDbContext _dbContext;
        private readonly IMemoryCache _cache;
        private const string ItemAuditsCacheKey = "itemAuditsCache";
        private const string ItemAuditCacheKeyPrefix = "itemAuditCache_";

        public ItemAuditRepository(SiceDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<Result<IEnumerable<ItemAudit>>> GetAllItemAuditsAsync()
        {
            if (!_cache.TryGetValue(ItemAuditsCacheKey, out IEnumerable<ItemAudit> itemAudits))
            {
                itemAudits = await _dbContext.ItemAudits.AsNoTracking().ToListAsync();
                _cache.Set(ItemAuditsCacheKey, itemAudits, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1)));
            }
            return Result<IEnumerable<ItemAudit>>.Success(itemAudits);
        }

        public async Task<Result> AddItemAuditAsync(ItemAudit itemAudit)
        {
            await _dbContext.ItemAudits.AddAsync(itemAudit);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(ItemAuditsCacheKey);
            return Result.Success();
        }

        public async Task<Result<ItemAudit>> GetItemAuditByIdAsync(int id)
        {
            var cacheKey = $"{ItemAuditCacheKeyPrefix}{id}";
            if (!_cache.TryGetValue(cacheKey, out ItemAudit itemAudit))
            {
                itemAudit = await _dbContext.ItemAudits.AsNoTracking().FirstOrDefaultAsync(ia => ia.Id == id);
                if (itemAudit == null)
                    return Result<ItemAudit>.Failure(ItemAuditErrors.ItemAuditNotFoundById(id));

                _cache.Set(cacheKey, itemAudit, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1)));
            }
            return Result<ItemAudit>.Success(itemAudit);
        }

        public async Task<Result<IEnumerable<ItemAudit>>> GetItemAuditsByItemIdAsync(int itemId)
        {
            var itemAudits = await _dbContext.ItemAudits.AsNoTracking()
                .Where(ia => ia.ItemId == itemId)
                .ToListAsync();
            return Result<IEnumerable<ItemAudit>>.Success(itemAudits);
        }

        public async Task<Result<IEnumerable<ItemAudit>>> GetItemAuditsByChangedByUserAsync(int userInfoId)
        {
            var itemAudits = await _dbContext.ItemAudits.AsNoTracking()
                .Where(ia => ia.ChangedBy == userInfoId)
                .ToListAsync();
            return Result<IEnumerable<ItemAudit>>.Success(itemAudits);
        }

        public async Task<Result> UpdateItemAuditAsync(ItemAudit itemAudit)
        {
            var existingItemAudit = await _dbContext.ItemAudits.FindAsync(itemAudit.Id);
            if (existingItemAudit == null)
                return Result.Failure(ItemAuditErrors.ItemAuditNotFoundById(itemAudit.Id));

            _dbContext.Entry(existingItemAudit).CurrentValues.SetValues(itemAudit);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(ItemAuditsCacheKey);
            _cache.Remove($"{ItemAuditCacheKeyPrefix}{itemAudit.Id}");
            return Result.Success();
        }

        public async Task<Result> DeleteItemAuditAsync(int id)
        {
            var itemAudit = await _dbContext.ItemAudits.FindAsync(id);
            if (itemAudit == null)
                return Result.Failure(ItemAuditErrors.ItemAuditNotFoundById(id));

            _dbContext.ItemAudits.Remove(itemAudit);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(ItemAuditsCacheKey);
            _cache.Remove($"{ItemAuditCacheKeyPrefix}{id}");
            return Result.Success();
        }
    }
}
