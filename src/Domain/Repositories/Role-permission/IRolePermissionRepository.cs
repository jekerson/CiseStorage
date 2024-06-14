using Domain.Abstraction;
using Domain.Entities;

namespace Domain.Repositories.Role_permission
{
    public interface IRolePermissionRepository
    {
        Task<Result<IEnumerable<RolePermission>>> GetAllRolePermissionsAsync();
        Task<Result> AddRolePermissionAsync(RolePermission rolePermission);
        Task<Result<RolePermission>> GetRolePermissionByIdAsync(int id);
        Task<Result<IEnumerable<RolePermission>>> GetRolePermissionsByRoleIdAsync(int roleId);
        Task<Result<IEnumerable<RolePermission>>> GetRolePermissionsByPermissionIdAsync(int permissionId);
        Task<Result> DeleteRolePermissionAsync(int id);
    }

}
