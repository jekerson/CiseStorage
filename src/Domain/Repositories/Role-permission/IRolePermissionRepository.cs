using Domain.Abstraction;
using Domain.Entities;

namespace Domain.Repositories.Role_permission
{
    public interface IRolePermissionRepository
    {
        Task<Result<IEnumerable<RolePermission>>> GetAllRolePermissionsAsync();
        Task<Result> AddRolePermissionAsync(RolePermission rolePermission);
        Task<Result<RolePermission>> GetRolePermissionByIdAsync(int id);
        Task<Result<IEnumerable<Permission>>> GetPermissionsByRoleIdAsync(int roleId);
        Task<Result<IEnumerable<Role>>> GetRolesByPermissionIdAsync(int permissionId);
        Task<Result> DeleteRolePermissionAsync(int id);
    }
}
