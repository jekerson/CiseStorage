using Domain.Abstraction;
using Domain.Entities;

namespace Domain.Repositories.Role_permission
{
    public interface IPermissionRepository
    {
        Task<Result<IEnumerable<Permission>>> GetAllPermissionsAsync();
        Task<Result> AddPermissionAsync(Permission permission);
        Task<Result<Permission>> GetPermissionByIdAsync(int id);
        Task<Result<Permission>> GetPermissionByNameAsync(string name);
        Task<Result> UpdatePermissionAsync(Permission permission);
        Task<Result> DeletePermissionAsync(int id);
        Task<Result<IEnumerable<Permission>>> GetPermissionsByUserIdAsync(int userId);
    }
}
