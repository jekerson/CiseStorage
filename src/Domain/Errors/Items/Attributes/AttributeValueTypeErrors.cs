using Domain.Abstraction;

namespace Domain.Errors.Items.Attributes
{
    public static class AttributeValueTypeErrors
    {
        public static Error AttributeValueTypeAlreadyExist(string name) =>
            Error.Conflict(
                "AttributeValueType.AlreadyExist",
                $"Attribute value type with name '{name}' already exists.");

        public static Error AttributeValueTypeNotFoundById(int id) =>
            Error.NotFound(
                "AttributeValueType.NotFoundById",
                $"Attribute value type with ID '{id}' not found.");

        public static Error AttributeValueTypeNotFoundByName(string name) =>
            Error.NotFound(
                "AttributeValueType.NotFoundByName",
                $"Attribute value type with name '{name}' not found.");
    }
}
