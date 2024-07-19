using Application.Abstraction.Cache;
using Domain.Abstraction;
using Domain.Entities;
using Domain.Errors;
using Domain.Repositories.Role_permission;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Role_permission
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly SiceDbContext _dbContext;
        private readonly ICacheProvider _cacheProvider;
        private const string PermissionsCacheKey = "permissionsCache";
        private const string PermissionCacheKeyPrefix = "permissionCache_";
        public PermissionRepository(SiceDbContext dbContext, ICacheProvider cacheProvider)
        {
            _dbContext = dbContext;
            _cacheProvider = cacheProvider;
        }

        public async Task<Result<IEnumerable<Permission>>> GetAllPermissionsAsync()
        {
            var permissions = await _cacheProvider.GetAsync<IEnumerable<Permission>>(PermissionsCacheKey);
            if (permissions == null)
            {
                permissions = await _dbContext.Permissions.AsNoTracking().ToListAsync();
                await _cacheProvider.SetAsync(PermissionsCacheKey, permissions, TimeSpan.FromHours(1));
            }
            return Result<IEnumerable<Permission>>.Success(permissions);
        }

        public async Task<Result> AddPermissionAsync(Permission permission)
        {
            if (await IsPermissionExistByNameAsync(permission.Name))
                return Result.Failure(PermissionErrors.PermissionAlreadyExist(permission.Name));

            await _dbContext.Permissions.AddAsync(permission);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(PermissionsCacheKey);
            return Result.Success();
        }

        public async Task<Result<Permission>> GetPermissionByIdAsync(int id)
        {
            var cacheKey = $"{PermissionCacheKeyPrefix}{id}";
            var permission = await _cacheProvider.GetAsync<Permission>(cacheKey);
            if (permission == null)
            {
                permission = await _dbContext.Permissions.FindAsync(id);
                if (permission == null)
                    return Result<Permission>.Failure(PermissionErrors.PermissionNotFoundById(id));

                await _cacheProvider.SetAsync(cacheKey, permission, TimeSpan.FromHours(1));
            }
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
            await _cacheProvider.RemoveAsync(PermissionsCacheKey);
            await _cacheProvider.RemoveAsync($"{PermissionCacheKeyPrefix}{permission.Id}");
            return Result.Success();
        }

        public async Task<Result> DeletePermissionAsync(int id)
        {
            var permission = await _dbContext.Permissions.FindAsync(id);
            if (permission == null)
                return Result.Failure(PermissionErrors.PermissionNotFoundById(id));

            _dbContext.Permissions.Remove(permission);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(PermissionsCacheKey);
            await _cacheProvider.RemoveAsync($"{PermissionCacheKeyPrefix}{id}");
            return Result.Success();
        }

        public async Task<Result<IEnumerable<Permission>>> GetPermissionsByUserIdAsync(int userId)
        {
            var permissions = await _dbContext.UserRoles
                    .Where(ur => ur.UserInfoId == userId)
                    .SelectMany(ur => ur.Role.RolePermissions)
                    .Select(rp => rp.Permission)
                    .Distinct()
                    .AsNoTracking()
                    .ToListAsync();

            return Result<IEnumerable<Permission>>.Success(permissions);
        }

        private async Task<bool> IsPermissionExistByNameAsync(string name)
        {
            return await _dbContext.Permissions.AnyAsync(p => p.Name == name);
        }
    }
}
