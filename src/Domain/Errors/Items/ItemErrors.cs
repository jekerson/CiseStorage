using Domain.Abstraction;

namespace Domain.Errors.Items
{
    public static class ItemErrors
    {
        public static Error ItemAlreadyExistByName(string name) =>
            Error.Conflict(
                "Item.AlreadyExistByName",
                $"Item with name '{name}' already exists.");

        public static Error ItemAlreadyExistByNumber(string number) =>
            Error.Conflict(
                "Item.AlreadyExistByNumber",
                $"Item with number '{number}' already exists.");

        public static Error ItemNotFoundById(int id) =>
            Error.NotFound(
                "Item.NotFoundById",
                $"Item with ID '{id}' not found.");

        public static Error ItemNotFoundByName(string name) =>
            Error.NotFound(
                "Item.NotFoundByName",
                $"Item with name '{name}' not found.");

        public static Error ItemNotFoundByNumber(string number) =>
            Error.NotFound(
                "Item.NotFoundByNumber",
                $"Item with number '{number}' not found.");
    }
}
