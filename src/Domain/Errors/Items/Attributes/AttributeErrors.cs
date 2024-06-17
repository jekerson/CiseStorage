using Domain.Abstraction;

namespace Domain.Errors.Items.Attributes
{
    public static class AttributeErrors
    {
        public static Error AttributeAlreadyExist(string name) =>
            Error.Conflict(
                "Attribute.AlreadyExist",
                $"Attribute with name '{name}' already exists.");

        public static Error AttributeNotFoundById(int id) =>
            Error.NotFound(
                "Attribute.NotFoundById",
                $"Attribute with ID '{id}' not found.");

        public static Error AttributeNotFoundByName(string name) =>
            Error.NotFound(
                "Attribute.NotFoundByName",
                $"Attribute with name '{name}' not found.");
    }
}
