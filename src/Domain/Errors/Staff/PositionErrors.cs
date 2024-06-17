using Domain.Abstraction;

namespace Domain.Errors.Staff
{
    public static class PositionErrors
    {
        public static Error PositionAlreadyExist(string name) =>
            Error.Conflict(
                "Position.AlreadyExist",
                $"Position with name '{name}' already exists.");

        public static Error PositionNotFoundById(int id) =>
            Error.NotFound(
                "Position.NotFoundById",
                $"Position with ID '{id}' not found.");

        public static Error PositionNotFoundByName(string name) =>
            Error.NotFound(
                "Position.NotFoundByName",
                $"Position with name '{name}' not found.");
    }
}
