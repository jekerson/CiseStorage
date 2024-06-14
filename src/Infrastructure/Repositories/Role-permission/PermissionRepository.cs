using Domain.Abstraction;
using Domain.Entities;
using Domain.Errors;
using Domain.Repositories.Role_permission;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Repositories.Role_permission
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly SiceDbContext _dbContext;
        private readonly IMemoryCache _cache;
        private const string PermissionsCacheKey = "permissionsCache";

        public PermissionRepository(SiceDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<Result<IEnumerable<Permission>>> GetAllPermissionsAsync()
        {
            if (!_cache.TryGetValue(PermissionsCacheKey, out IEnumerable<Permission> permissions))
            {
                permissions = await _dbContext.Permissions.AsNoTracking().ToListAsync();
                _cache.Set(PermissionsCacheKey, permissions, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1)));
            }
            return Result<IEnumerable<Permission>>.Success(permissions);
        }

        public async Task<Result> AddPermissionAsync(Permission permission)
        {
            if (await IsPermissionExistByNameAsync(permission.Name))
                return Result.Failure(PermissionErrors.PermissionAlreadyExist(permission.Name));

            await _dbContext.Permissions.AddAsync(permission);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(PermissionsCacheKey);
            return Result.Success();
        }

        public async Task<Result<Permission>> GetPermissionByIdAsync(int id)
        {
            var permission = await _dbContext.Permissions.FindAsync(id);
            if (permission == null)
                return Result<Permission>.Failure(PermissionErrors.PermissionNotFoundById(id));

            return Result<Permission>.Success(permission);
        }

        public async Task<Result<Permission>> GetPermissionByNameAsync(string name)
        {
            var permission = await _dbContext.Permissions.AsNoTracking().FirstOrDefaultAsync(p => p.Name == name);
            if (permission == null)
                return Result<Permission>.Failure(PermissionErrors.PermissionNotFoundByName(name));

            return Result<Permission>.Success(permission);
        }

        public async Task<Result> UpdatePermissionAsync(Permission permission)
        {
            var existingPermission = await _dbContext.Permissions.FindAsync(permission.Id);
            if (existingPermission == null)
                return Result.Failure(PermissionErrors.PermissionNotFoundById(permission.Id));

            if (existingPermission.Name != permission.Name && await IsPermissionExistByNameAsync(permission.Name))
                return Result.Failure(PermissionErrors.PermissionAlreadyExist(permission.Name));

            _dbContext.Entry(existingPermission).CurrentValues.SetValues(permission);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(PermissionsCacheKey);
            return Result.Success();
        }

        public async Task<Result> DeletePermissionAsync(int id)
        {
            var permission = await _dbContext.Permissions.FindAsync(id);
            if (permission == null)
                return Result.Failure(PermissionErrors.PermissionNotFoundById(id));

            _dbContext.Permissions.Remove(permission);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(PermissionsCacheKey);
            return Result.Success();
        }

        private async Task<bool> IsPermissionExistByNameAsync(string name)
        {
            return await _dbContext.Permissions.AnyAsync(p => p.Name == name);
        }
    }
}
