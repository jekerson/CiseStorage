using Domain.Abstraction;
using Domain.Entities;
using Domain.Errors;
using Domain.Repositories.Role_permission;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Application.Abstraction.Cache;

namespace Infrastructure.Repositories.Role_permission
{
    public class RolePermissionRepository : IRolePermissionRepository
    {
        private readonly SiceDbContext _dbContext;
        private readonly ICacheProvider _cacheProvider;
        private const string RolePermissionsCacheKey = "rolePermissionsCache";
        private const string RolePermissionCacheKeyPrefix = "rolePermissionCache_";

        public RolePermissionRepository(SiceDbContext dbContext, ICacheProvider cacheProvider)
        {
            _dbContext = dbContext;
            _cacheProvider = cacheProvider;
        }

        public async Task<Result<IEnumerable<RolePermission>>> GetAllRolePermissionsAsync()
        {
            var rolePermissions = await _cacheProvider.GetAsync<IEnumerable<RolePermission>>(RolePermissionsCacheKey);
            if (rolePermissions == null)
            {
                rolePermissions = await _dbContext.RolePermissions.AsNoTracking().ToListAsync();
                await _cacheProvider.SetAsync(RolePermissionsCacheKey, rolePermissions, TimeSpan.FromHours(1));
            }
            return Result<IEnumerable<RolePermission>>.Success(rolePermissions);
        }

        public async Task<Result> AddRolePermissionAsync(RolePermission rolePermission)
        {
            if (await IsRolePermissionExistAsync(rolePermission.RoleId, rolePermission.PermissionId))
                return Result.Failure(RolePermissionErrors.RolePermissionAlreadyExist(rolePermission.RoleId, rolePermission.PermissionId));

            await _dbContext.RolePermissions.AddAsync(rolePermission);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(RolePermissionsCacheKey);
            return Result.Success();
        }

        public async Task<Result<RolePermission>> GetRolePermissionByIdAsync(int id)
        {
            var cacheKey = $"{RolePermissionCacheKeyPrefix}{id}";
            var rolePermission = await _cacheProvider.GetAsync<RolePermission>(cacheKey);
            if (rolePermission == null)
            {
                rolePermission = await _dbContext.RolePermissions.FindAsync(id);
                if (rolePermission == null)
                    return Result<RolePermission>.Failure(RolePermissionErrors.RolePermissionNotFoundById(id));

                await _cacheProvider.SetAsync(cacheKey, rolePermission, TimeSpan.FromHours(1));
            }
            return Result<RolePermission>.Success(rolePermission);
        }

        public async Task<Result<IEnumerable<RolePermission>>> GetRolePermissionsByRoleIdAsync(int roleId)
        {
            var rolePermissions = await _dbContext.RolePermissions.AsNoTracking().Where(rp => rp.RoleId == roleId).ToListAsync();
            return Result<IEnumerable<RolePermission>>.Success(rolePermissions);
        }

        public async Task<Result<IEnumerable<RolePermission>>> GetRolePermissionsByPermissionIdAsync(int permissionId)
        {
            var rolePermissions = await _dbContext.RolePermissions.AsNoTracking().Where(rp => rp.PermissionId == permissionId).ToListAsync();
            return Result<IEnumerable<RolePermission>>.Success(rolePermissions);
        }

        public async Task<Result> DeleteRolePermissionAsync(int id)
        {
            var rolePermission = await _dbContext.RolePermissions.FindAsync(id);
            if (rolePermission == null)
                return Result.Failure(RolePermissionErrors.RolePermissionNotFoundById(id));

            _dbContext.RolePermissions.Remove(rolePermission);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(RolePermissionsCacheKey);
            await _cacheProvider.RemoveAsync($"{RolePermissionCacheKeyPrefix}{id}");
            return Result.Success();
        }

        private async Task<bool> IsRolePermissionExistAsync(int roleId, int permissionId)
        {
            return await _dbContext.RolePermissions.AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);
        }
    }
}
