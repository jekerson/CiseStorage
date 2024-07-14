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
    public class RoleRepository : IRoleRepository
    {
        private readonly SiceDbContext _dbContext;
        private readonly ICacheProvider _cacheProvider;
        private const string RolesCacheKey = "rolesCache";

        public RoleRepository(SiceDbContext dbContext, ICacheProvider cacheProvider)
        {
            _dbContext = dbContext;
            _cacheProvider = cacheProvider;
        }

        public async Task<Result<IEnumerable<Role>>> GetAllRolesAsync()
        {
            var roles = await _cacheProvider.GetAsync<IEnumerable<Role>>(RolesCacheKey);
            if (roles == null)
            {
                roles = await _dbContext.Roles.AsNoTracking().ToListAsync();
                await _cacheProvider.SetAsync(RolesCacheKey, roles, TimeSpan.FromHours(1));
            }
            return Result<IEnumerable<Role>>.Success(roles);
        }

        public async Task<Result> AddRoleAsync(Role role)
        {
            if (await IsRoleExistByNameAsync(role.Name))
                return Result.Failure(RoleErrors.RoleAlreadyExist(role.Name));

            await _dbContext.Roles.AddAsync(role);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(RolesCacheKey);
            return Result.Success();
        }

        public async Task<Result<Role>> GetRoleByIdAsync(int id)
        {
            var role = await _dbContext.Roles.FindAsync(id);
            if (role == null)
                return Result<Role>.Failure(RoleErrors.RoleNotFoundById(id));

            return Result<Role>.Success(role);
        }

        public async Task<Result<Role>> GetRoleByNameAsync(string name)
        {
            var role = await _dbContext.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Name == name);
            if (role == null)
                return Result<Role>.Failure(RoleErrors.RoleNotFoundByName(name));

            return Result<Role>.Success(role);
        }

        public async Task<Result> UpdateRoleAsync(Role role)
        {
            var existingRole = await _dbContext.Roles.FindAsync(role.Id);
            if (existingRole == null)
                return Result.Failure(RoleErrors.RoleNotFoundById(role.Id));

            if (existingRole.Name != role.Name && await IsRoleExistByNameAsync(role.Name))
                return Result.Failure(RoleErrors.RoleAlreadyExist(role.Name));

            _dbContext.Entry(existingRole).CurrentValues.SetValues(role);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(RolesCacheKey);
            return Result.Success();
        }

        public async Task<Result> DeleteRoleAsync(int id)
        {
            var role = await _dbContext.Roles.FindAsync(id);
            if (role == null)
                return Result.Failure(RoleErrors.RoleNotFoundById(id));

            _dbContext.Roles.Remove(role);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(RolesCacheKey);
            return Result.Success();
        }

        private async Task<bool> IsRoleExistByNameAsync(string name)
        {
            return await _dbContext.Roles.AnyAsync(r => r.Name == name);
        }
    }
}
