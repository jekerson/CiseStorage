using Domain.Abstraction;
using Domain.Entities;
using Domain.Errors;
using Domain.Repositories.Role_permission;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Role_permission
{
    public class RolePermissionRepository : IRolePermissionRepository
    {
        private readonly SiceDbContext _dbContext;

        public RolePermissionRepository(SiceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<IEnumerable<RolePermission>>> GetAllRolePermissionsAsync()
        {
            var rolePermissions = await _dbContext.RolePermissions
                .Include(rp => rp.Role)
                .Include(rp => rp.Permission)
                .AsNoTracking()
                .ToListAsync();

            return Result<IEnumerable<RolePermission>>.Success(rolePermissions);
        }

        public async Task<Result> AddRolePermissionAsync(RolePermission rolePermission)
        {
            if (await IsRolePermissionExistAsync(rolePermission.RoleId, rolePermission.PermissionId))
                return Result.Failure(RolePermissionErrors.RolePermissionAlreadyExist(rolePermission.RoleId, rolePermission.PermissionId));

            await _dbContext.RolePermissions.AddAsync(rolePermission);
            await _dbContext.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result<RolePermission>> GetRolePermissionByIdAsync(int id)
        {
            var rolePermission = await _dbContext.RolePermissions
                .Include(rp => rp.Role)
                .Include(rp => rp.Permission)
                .AsNoTracking()
                .FirstOrDefaultAsync(rp => rp.Id == id);

            if (rolePermission == null)
                return Result<RolePermission>.Failure(RolePermissionErrors.RolePermissionNotFoundById(id));

            return Result<RolePermission>.Success(rolePermission);
        }

        public async Task<Result<IEnumerable<Permission>>> GetPermissionsByRoleIdAsync(int roleId)
        {
            var permissions = await _dbContext.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Include(rp => rp.Permission)
                .Select(rp => rp.Permission)
                .AsNoTracking()
                .ToListAsync();

            return Result<IEnumerable<Permission>>.Success(permissions);
        }

        public async Task<Result<IEnumerable<Role>>> GetRolesByPermissionIdAsync(int permissionId)
        {
            var roles = await _dbContext.RolePermissions
                .Where(rp => rp.PermissionId == permissionId)
                .Include(rp => rp.Role)
                .Select(rp => rp.Role)
                .AsNoTracking()
                .ToListAsync();

            return Result<IEnumerable<Role>>.Success(roles);
        }

        public async Task<Result> DeleteRolePermissionAsync(int id)
        {
            var rolePermission = await _dbContext.RolePermissions.FindAsync(id);
            if (rolePermission == null)
                return Result.Failure(RolePermissionErrors.RolePermissionNotFoundById(id));

            _dbContext.RolePermissions.Remove(rolePermission);
            await _dbContext.SaveChangesAsync();
            return Result.Success();
        }

        private async Task<bool> IsRolePermissionExistAsync(int roleId, int permissionId)
        {
            return await _dbContext.RolePermissions.AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);
        }
    }

}
