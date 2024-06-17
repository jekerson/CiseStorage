using Domain.Abstraction;

namespace Domain.Errors.Items.Attributes
{
    public static class ItemAttributeValueErrors
    {
        public static Error ItemAttributeValueNotFoundById(int id) =>
            Error.NotFound(
                "ItemAttributeValue.NotFoundById",
                $"Item attribute value with ID '{id}' not found.");
    }
}
