using Domain.Abstraction;

namespace Domain.Errors
{
    public static class RefreshTokenErrors
    {
        public static Error TokenAlreadyExist(string token) =>
            Error.Conflict(
                "RefreshToken.AlreadyExist",
                $"Refresh token '{token}' already exists.");

        public static Error TokenNotFoundById(int id) =>
            Error.NotFound(
                "RefreshToken.NotFoundById",
                $"Refresh token with ID '{id}' not found.");

        public static Error TokenNotFoundByToken(string token) =>
            Error.NotFound(
                "RefreshToken.NotFoundByToken",
                $"Refresh token '{token}' not found.");
    }
}
