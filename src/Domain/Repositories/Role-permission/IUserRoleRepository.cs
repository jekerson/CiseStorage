using Domain.Abstraction;
using Domain.Entities;

namespace Domain.Repositories.Role_permission
{
    public interface IUserRoleRepository
    {
        Task<Result<IEnumerable<UserRole>>> GetAllUserRolesAsync();
        Task<Result> AddUserRoleAsync(UserRole userRole);
        Task<Result<UserRole>> GetUserRoleByIdAsync(int id);
        Task<Result<IEnumerable<UserRole>>> GetUserRolesByUserIdAsync(int userId);
        Task<Result<IEnumerable<UserRole>>> GetUserRolesByRoleIdAsync(int roleId);
        Task<Result> DeleteUserRoleAsync(int id);
    }


}
