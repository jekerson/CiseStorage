using Domain.Abstraction;

namespace Domain.Errors
{
    public static class RolePermissionErrors
    {
        public static Error RolePermissionAlreadyExist(int roleId, int permissionId) =>
            Error.Conflict(
                "RolePermission.AlreadyExist",
                $"RolePermission with Role ID '{roleId}' and Permission ID '{permissionId}' already exists.");

        public static Error RolePermissionNotFoundById(int rolePermissionId) =>
            Error.NotFound(
                "RolePermission.NotFoundById",
                $"RolePermission with ID '{rolePermissionId}' not found.");
    }
}
