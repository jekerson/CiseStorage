using Domain.Abstraction;

namespace Domain.Errors
{
    public static class PermissionErrors
    {
        public static Error PermissionAlreadyExist(string permissionName) =>
            Error.Conflict(
                "Permission.AlreadyExist",
                $"Permission with name '{permissionName}' already exists.");

        public static Error PermissionNotFoundById(int permissionId) =>
            Error.NotFound(
                "Permission.NotFoundById",
                $"Permission with ID '{permissionId}' not found.");

        public static Error PermissionNotFoundByName(string permissionName) =>
            Error.NotFound(
                "Permission.NotFoundByName",
                $"Permission with name '{permissionName}' not found.");
    }
}
