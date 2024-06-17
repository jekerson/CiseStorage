using Domain.Abstraction;

namespace Domain.Errors.Addresses
{
    public static class RegionErrors
    {
        public static Error RegionAlreadyExist(string name) =>
            Error.Conflict(
                "Region.AlreadyExist",
                $"Region with name '{name}' already exists.");

        public static Error RegionNotFoundById(int id) =>
            Error.NotFound(
                "Region.NotFoundById",
                $"Region with ID '{id}' not found.");

        public static Error RegionNotFoundByName(string name) =>
            Error.NotFound(
                "Region.NotFoundByName",
                $"Region with name '{name}' not found.");
    }
}
