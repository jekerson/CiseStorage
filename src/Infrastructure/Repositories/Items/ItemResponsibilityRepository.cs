using Application.Abstraction.Cache;
using Domain.Abstraction;
using Domain.Entities;
using Domain.Errors.Items;
using Domain.Repositories.Item;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Items
{
    public class ItemResponsibilityRepository : IItemResponsibilityRepository
    {
        private readonly SiceDbContext _dbContext;
        private readonly ICacheProvider _cacheProvider;
        private const string ItemResponsibilitiesCacheKey = "itemResponsibilitiesCache";
        private const string ItemResponsibilityCacheKeyPrefix = "itemResponsibilityCache_";

        public ItemResponsibilityRepository(SiceDbContext dbContext, ICacheProvider cacheProvider)
        {
            _dbContext = dbContext;
            _cacheProvider = cacheProvider;
        }

        public async Task<Result<IEnumerable<ItemResponsibility>>> GetAllItemResponsibilitiesAsync()
        {
            var itemResponsibilities = await _cacheProvider.GetAsync<IEnumerable<ItemResponsibility>>(ItemResponsibilitiesCacheKey);
            if (itemResponsibilities == null)
            {
                itemResponsibilities = await _dbContext.ItemResponsibilities.AsNoTracking().ToListAsync();
                await _cacheProvider.SetAsync(ItemResponsibilitiesCacheKey, itemResponsibilities, TimeSpan.FromHours(1));
            }
            return Result<IEnumerable<ItemResponsibility>>.Success(itemResponsibilities);
        }

        public async Task<Result> AddItemResponsibilityAsync(ItemResponsibility itemResponsibility)
        {
            await _dbContext.ItemResponsibilities.AddAsync(itemResponsibility);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(ItemResponsibilitiesCacheKey);
            return Result.Success();
        }

        public async Task<Result<ItemResponsibility>> GetItemResponsibilityByIdAsync(int id)
        {
            var cacheKey = $"{ItemResponsibilityCacheKeyPrefix}{id}";
            var itemResponsibility = await _cacheProvider.GetAsync<ItemResponsibility>(cacheKey);
            if (itemResponsibility == null)
            {
                itemResponsibility = await _dbContext.ItemResponsibilities.AsNoTracking().FirstOrDefaultAsync(ir => ir.Id == id);
                if (itemResponsibility == null)
                    return Result<ItemResponsibility>.Failure(ItemResponsibilityErrors.ItemResponsibilityNotFoundById(id));

                await _cacheProvider.SetAsync(cacheKey, itemResponsibility, TimeSpan.FromHours(1));
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
            await _cacheProvider.RemoveAsync(ItemResponsibilitiesCacheKey);
            await _cacheProvider.RemoveAsync($"{ItemResponsibilityCacheKeyPrefix}{itemResponsibility.Id}");
            return Result.Success();
        }

        public async Task<Result> DeleteItemResponsibilityAsync(int id)
        {
            var itemResponsibility = await _dbContext.ItemResponsibilities.FindAsync(id);
            if (itemResponsibility == null)
                return Result.Failure(ItemResponsibilityErrors.ItemResponsibilityNotFoundById(id));

            _dbContext.ItemResponsibilities.Remove(itemResponsibility);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(ItemResponsibilitiesCacheKey);
            await _cacheProvider.RemoveAsync($"{ItemResponsibilityCacheKeyPrefix}{id}");
            return Result.Success();
        }
    }
}
