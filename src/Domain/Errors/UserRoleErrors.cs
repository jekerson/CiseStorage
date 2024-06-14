using Domain.Abstraction;

namespace Domain.Errors
{
    public static class UserRoleErrors
    {
        public static Error UserRoleAlreadyExist(int userId, int roleId) =>
            Error.Conflict(
                "UserRole.AlreadyExist",
                $"UserRole with User ID '{userId}' and Role ID '{roleId}' already exists.");

        public static Error UserRoleNotFoundById(int userRoleId) =>
            Error.NotFound(
                "UserRole.NotFoundById",
                $"UserRole with ID '{userRoleId}' not found.");
    }
}
