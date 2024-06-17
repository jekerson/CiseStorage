using Domain.Abstraction;

namespace Domain.Errors
{
    public static class RoleErrors
    {
        public static Error RoleAlreadyExist(string roleName) =>
            Error.Conflict(
                "Role.AlreadyExist",
                $"Role with name '{roleName}' already exists.");

        public static Error RoleNotFoundById(int roleId) =>
            Error.NotFound(
                "Role.NotFoundById",
                $"Role with ID '{roleId}' not found.");

        public static Error RoleNotFoundByName(string roleName) =>
            Error.NotFound(
                "RoleNotFoundByName",
                $"Role with name '{roleName}' not found.");
    }

}
