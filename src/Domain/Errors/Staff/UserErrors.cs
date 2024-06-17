using Domain.Abstraction;

namespace Domain.Errors.Staff
{
    public static class UserErrors
    {
        public static Error UserAlreadyExistByUsername(string username) =>
            Error.Conflict(
                "User.AlreadyExistByUsername",
                $"User with username '{username}' already exists.");

        public static Error UserNotFoundById(int userId) =>
            Error.NotFound(
                "User.NotFoundById",
                $"User with ID '{userId}' not found.");

        public static Error UserNotFoundByUsername(string username) =>
            Error.NotFound(
                "User.NotFoundByUsername",
                $"User with username '{username}' not found.");
    }
}
