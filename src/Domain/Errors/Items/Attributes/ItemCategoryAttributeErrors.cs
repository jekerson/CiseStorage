using Domain.Abstraction;

namespace Domain.Errors.Items.Attributes
{
    public static class ItemCategoryAttributeErrors
    {
        public static Error ItemCategoryAttributeNotFoundById(int id) =>
            Error.NotFound(
                "ItemCategoryAttribute.NotFoundById",
                $"Item category attribute with ID '{id}' not found.");
    }
}
