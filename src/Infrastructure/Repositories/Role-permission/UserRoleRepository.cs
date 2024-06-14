using Domain.Abstraction;
using Domain.Entities;
using Domain.Errors;
using Domain.Repositories.Role_permission;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Role_permission
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly SiceDbContext _dbContext;

        public UserRoleRepository(SiceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<IEnumerable<UserRole>>> GetAllUserRolesAsync()
        {
            var userRoles = await _dbContext.UserRoles.AsNoTracking().ToListAsync();
            return Result<IEnumerable<UserRole>>.Success(userRoles);
        }

        public async Task<Result> AddUserRoleAsync(UserRole userRole)
        {
            if (await IsUserRoleExistAsync(userRole.UserInfoId, userRole.RoleId))
                return Result.Failure(UserRoleErrors.UserRoleAlreadyExist(userRole.UserInfoId, userRole.RoleId));

            await _dbContext.UserRoles.AddAsync(userRole);
            await _dbContext.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result<UserRole>> GetUserRoleByIdAsync(int id)
        {
            var userRole = await _dbContext.UserRoles.FindAsync(id);
            if (userRole == null)
                return Result<UserRole>.Failure(UserRoleErrors.UserRoleNotFoundById(id));

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
            return Result.Success();
        }

        private async Task<bool> IsUserRoleExistAsync(int userId, int roleId)
        {
            return await _dbContext.UserRoles.AnyAsync(ur => ur.UserInfoId == userId && ur.RoleId == roleId);
        }
    }


}
