using Application.Abstraction.Cache;
using Domain.Abstraction;
using Domain.Entities;
using Domain.Errors.Audit;
using Domain.Repositories.Audit;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Audit
{
    public class ItemAuditRepository : IItemAuditRepository
    {
        private readonly SiceDbContext _dbContext;
        private readonly ICacheProvider _cacheProvider;
        private const string ItemAuditsCacheKey = "itemAuditsCache";
        private const string ItemAuditCacheKeyPrefix = "itemAuditCache_";

        public ItemAuditRepository(SiceDbContext dbContext, ICacheProvider cacheProvider)
        {
            _dbContext = dbContext;
            _cacheProvider = cacheProvider;
        }

        public async Task<Result<IEnumerable<ItemAudit>>> GetAllItemAuditsAsync()
        {
            var itemAudits = await _cacheProvider.GetAsync<IEnumerable<ItemAudit>>(ItemAuditsCacheKey);
            if (itemAudits == null)
            {
                itemAudits = await _dbContext.ItemAudits.AsNoTracking().ToListAsync();
                await _cacheProvider.SetAsync(ItemAuditsCacheKey, itemAudits, TimeSpan.FromHours(1));
            }
            return Result<IEnumerable<ItemAudit>>.Success(itemAudits);
        }

        public async Task<Result> AddItemAuditAsync(ItemAudit itemAudit)
        {
            await _dbContext.ItemAudits.AddAsync(itemAudit);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(ItemAuditsCacheKey);
            return Result.Success();
        }

        public async Task<Result<ItemAudit>> GetItemAuditByIdAsync(int id)
        {
            var cacheKey = $"{ItemAuditCacheKeyPrefix}{id}";
            var itemAudit = await _cacheProvider.GetAsync<ItemAudit>(cacheKey);
            if (itemAudit == null)
            {
                itemAudit = await _dbContext.ItemAudits.AsNoTracking().FirstOrDefaultAsync(ia => ia.Id == id);
                if (itemAudit == null)
                    return Result<ItemAudit>.Failure(ItemAuditErrors.ItemAuditNotFoundById(id));

                await _cacheProvider.SetAsync(cacheKey, itemAudit, TimeSpan.FromHours(1));
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
            await _cacheProvider.RemoveAsync(ItemAuditsCacheKey);
            await _cacheProvider.RemoveAsync($"{ItemAuditCacheKeyPrefix}{itemAudit.Id}");
            return Result.Success();
        }

        public async Task<Result> DeleteItemAuditAsync(int id)
        {
            var itemAudit = await _dbContext.ItemAudits.FindAsync(id);
            if (itemAudit == null)
                return Result.Failure(ItemAuditErrors.ItemAuditNotFoundById(id));

            _dbContext.ItemAudits.Remove(itemAudit);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(ItemAuditsCacheKey);
            await _cacheProvider.RemoveAsync($"{ItemAuditCacheKeyPrefix}{id}");
            return Result.Success();
        }
    }
}
