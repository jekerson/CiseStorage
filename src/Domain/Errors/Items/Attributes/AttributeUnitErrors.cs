using Domain.Abstraction;

namespace Domain.Errors.Items.Attributes
{
    public static class AttributeUnitErrors
    {
        public static Error AttributeUnitAlreadyExistByName(string name) =>
            Error.Conflict(
                "AttributeUnit.AlreadyExistByName",
                $"Attribute unit with name '{name}' already exists.");

        public static Error AttributeUnitAlreadyExistBySymbol(string symbol) =>
            Error.Conflict(
                "AttributeUnit.AlreadyExistBySymbol",
                $"Attribute unit with symbol '{symbol}' already exists.");

        public static Error AttributeUnitNotFoundById(int id) =>
            Error.NotFound(
                "AttributeUnit.NotFoundById",
                $"Attribute unit with ID '{id}' not found.");

        public static Error AttributeUnitNotFoundByName(string name) =>
            Error.NotFound(
                "AttributeUnit.NotFoundByName",
                $"Attribute unit with name '{name}' not found.");

        public static Error AttributeUnitNotFoundBySymbol(string symbol) =>
            Error.NotFound(
                "AttributeUnit.NotFoundBySymbol",
                $"Attribute unit with symbol '{symbol}' not found.");
    }
}
