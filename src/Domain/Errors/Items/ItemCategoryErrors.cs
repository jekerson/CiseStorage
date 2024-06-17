using Domain.Abstraction;

namespace Domain.Errors.Items
{
    public static class ItemCategoryErrors
    {
        public static Error ItemCategoryAlreadyExist(string name) =>
            Error.Conflict(
                "ItemCategory.AlreadyExist",
                $"Item category with name '{name}' already exists.");

        public static Error ItemCategoryNotFoundById(int id) =>
            Error.NotFound(
                "ItemCategory.NotFoundById",
                $"Item category with ID '{id}' not found.");

        public static Error ItemCategoryNotFoundByName(string name) =>
            Error.NotFound(
                "ItemCategory.NotFoundByName",
                $"Item category with name '{name}' not found.");
    }
}
