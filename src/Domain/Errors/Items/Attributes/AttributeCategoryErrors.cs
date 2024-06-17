using Domain.Abstraction;

namespace Domain.Errors.Items.Attributes
{
    public static class AttributeCategoryErrors
    {
        public static Error AttributeCategoryAlreadyExist(string name) =>
            Error.Conflict(
                "AttributeCategory.AlreadyExist",
                $"Attribute category with name '{name}' already exists.");

        public static Error AttributeCategoryNotFoundById(int id) =>
            Error.NotFound(
                "AttributeCategory.NotFoundById",
                $"Attribute category with ID '{id}' not found.");

        public static Error AttributeCategoryNotFoundByName(string name) =>
            Error.NotFound(
                "AttributeCategory.NotFoundByName",
                $"Attribute category with name '{name}' not found.");
    }
}
