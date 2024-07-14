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
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly SiceDbContext _dbContext;
        private readonly ICacheProvider _cacheProvider;
        private const string UserRolesCacheKey = "userRolesCache";
        private const string UserRoleCacheKeyPrefix = "userRoleCache_";

        public UserRoleRepository(SiceDbContext dbContext, ICacheProvider cacheProvider)
        {
            _dbContext = dbContext;
            _cacheProvider = cacheProvider;
        }

        public async Task<Result<IEnumerable<UserRole>>> GetAllUserRolesAsync()
        {
            var userRoles = await _cacheProvider.GetAsync<IEnumerable<UserRole>>(UserRolesCacheKey);
            if (userRoles == null)
            {
                userRoles = await _dbContext.UserRoles.AsNoTracking().ToListAsync();
                await _cacheProvider.SetAsync(UserRolesCacheKey, userRoles, TimeSpan.FromHours(1));
            }
            return Result<IEnumerable<UserRole>>.Success(userRoles);
        }

        public async Task<Result> AddUserRoleAsync(UserRole userRole)
        {
            if (await IsUserRoleExistAsync(userRole.UserInfoId, userRole.RoleId))
                return Result.Failure(UserRoleErrors.UserRoleAlreadyExist(userRole.UserInfoId, userRole.RoleId));

            await _dbContext.UserRoles.AddAsync(userRole);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(UserRolesCacheKey);
            return Result.Success();
        }

        public async Task<Result<UserRole>> GetUserRoleByIdAsync(int id)
        {
            var cacheKey = $"{UserRoleCacheKeyPrefix}{id}";
            var userRole = await _cacheProvider.GetAsync<UserRole>(cacheKey);
            if (userRole == null)
            {
                userRole = await _dbContext.UserRoles.FindAsync(id);
                if (userRole == null)
                    return Result<UserRole>.Failure(UserRoleErrors.UserRoleNotFoundById(id));

                await _cacheProvider.SetAsync(cacheKey, userRole, TimeSpan.FromHours(1));
            }
            return Result<UserRole>.Success(userRole);
        }

        public async Task<Result<IEnumerable<UserRole>>> GetUserRolesByUserIdAsync(int userId)
        {
            var userRoles = await _dbContext.UserRoles.AsNoTracking().Where(ur => ur.UserInfoId == userId).ToListAsync();
            return Result<IEnumerable<UserRole>>.Success(userRoles);
        }

        public async Task<Result<IEnumerable<UserRole>>> GetUserRolesByRoleIdAsync(int roleId)
        {
            var userRoles = await _dbContext.UserRoles.AsNoTracking().Where(ur => ur.RoleId == roleId).ToListAsync();
            return Result<IEnumerable<UserRole>>.Success(userRoles);
        }

        public async Task<Result> DeleteUserRoleAsync(int id)
        {
            var userRole = await _dbContext.UserRoles.FindAsync(id);
            if (userRole == null)
                return Result.Failure(UserRoleErrors.UserRoleNotFoundById(id));

            _dbContext.UserRoles.Remove(userRole);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(UserRolesCacheKey);
            await _cacheProvider.RemoveAsync($"{UserRoleCacheKeyPrefix}{id}");
            return Result.Success();
        }

        private async Task<bool> IsUserRoleExistAsync(int userId, int roleId)
        {
            return await _dbContext.UserRoles.AnyAsync(ur => ur.UserInfoId == userId && ur.RoleId == roleId);
        }
    }
}
