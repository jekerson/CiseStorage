using Domain.Abstraction;
using Domain.Entities;
using Domain.Errors.Items;
using Domain.Repositories.Item;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Repositories.Items
{
    public class ItemResponsibilityRepository : IItemResponsibilityRepository
    {
        private readonly SiceDbContext _dbContext;
        private readonly IMemoryCache _cache;
        private const string ItemResponsibilitiesCacheKey = "itemResponsibilitiesCache";
        private const string ItemResponsibilityCacheKeyPrefix = "itemResponsibilityCache_";

        public ItemResponsibilityRepository(SiceDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<Result<IEnumerable<ItemResponsibility>>> GetAllItemResponsibilitiesAsync()
        {
            if (!_cache.TryGetValue(ItemResponsibilitiesCacheKey, out IEnumerable<ItemResponsibility> itemResponsibilities))
            {
                itemResponsibilities = await _dbContext.ItemResponsibilities.AsNoTracking().ToListAsync();
                _cache.Set(ItemResponsibilitiesCacheKey, itemResponsibilities, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1)));
            }
            return Result<IEnumerable<ItemResponsibility>>.Success(itemResponsibilities);
        }

        public async Task<Result> AddItemResponsibilityAsync(ItemResponsibility itemResponsibility)
        {
            await _dbContext.ItemResponsibilities.AddAsync(itemResponsibility);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(ItemResponsibilitiesCacheKey);
            return Result.Success();
        }

        public async Task<Result<ItemResponsibility>> GetItemResponsibilityByIdAsync(int id)
        {
            var cacheKey = $"{ItemResponsibilityCacheKeyPrefix}{id}";
            if (!_cache.TryGetValue(cacheKey, out ItemResponsibility itemResponsibility))
            {
                itemResponsibility = await _dbContext.ItemResponsibilities.AsNoTracking().FirstOrDefaultAsync(ir => ir.Id == id);
                if (itemResponsibility == null)
                    return Result<ItemResponsibility>.Failure(ItemResponsibilityErrors.ItemResponsibilityNotFoundById(id));

                _cache.Set(cacheKey, itemResponsibility, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1)));
            }
            return Result<ItemResponsibility>.Success(itemResponsibility);
        }

        public async Task<Result<IEnumerable<ItemResponsibility>>> GetItemResponsibilitiesByItemIdAsync(int itemId)
        {
            var itemResponsibilities = await _dbContext.ItemResponsibilities.AsNoTracking().Where(ir => ir.ItemId == itemId).ToListAsync();
            return Result<IEnumerable<ItemResponsibility>>.Success(itemResponsibilities);
        }

        public async Task<Result<IEnumerable<ItemResponsibility>>> GetItemResponsibilitiesByEmployeeIdAsync(int employeeId)
        {
            var itemResponsibilities = await _dbContext.ItemResponsibilities.AsNoTracking().Where(ir => ir.EmployeeId == employeeId).ToListAsync();
            return Result<IEnumerable<ItemResponsibility>>.Success(itemResponsibilities);
        }

        public async Task<Result> UpdateItemResponsibilityAsync(ItemResponsibility itemResponsibility)
        {
            var existingItemResponsibility = await _dbContext.ItemResponsibilities.FindAsync(itemResponsibility.Id);
            if (existingItemResponsibility == null)
                return Result.Failure(ItemResponsibilityErrors.ItemResponsibilityNotFoundById(itemResponsibility.Id));

            _dbContext.Entry(existingItemResponsibility).CurrentValues.SetValues(itemResponsibility);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(ItemResponsibilitiesCacheKey);
            _cache.Remove($"{ItemResponsibilityCacheKeyPrefix}{itemResponsibility.Id}");
            return Result.Success();
        }

        public async Task<Result> DeleteItemResponsibilityAsync(int id)
        {
            var itemResponsibility = await _dbContext.ItemResponsibilities.FindAsync(id);
            if (itemResponsibility == null)
                return Result.Failure(ItemResponsibilityErrors.ItemResponsibilityNotFoundById(id));

            _dbContext.ItemResponsibilities.Remove(itemResponsibility);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(ItemResponsibilitiesCacheKey);
            _cache.Remove($"{ItemResponsibilityCacheKeyPrefix}{id}");
            return Result.Success();
        }
    }
}
