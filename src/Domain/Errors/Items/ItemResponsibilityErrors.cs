using Domain.Abstraction;

namespace Domain.Errors.Items
{
    public static class ItemResponsibilityErrors
    {
        public static Error ItemResponsibilityNotFoundById(int id) =>
            Error.NotFound(
                "ItemResponsibility.NotFoundById",
                $"Item responsibility with ID '{id}' not found.");
    }
}
