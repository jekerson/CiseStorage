using Domain.Abstraction;

namespace Domain.Errors.Audit
{
    public static class ItemAuditErrors
    {
        public static Error ItemAuditNotFoundById(int id) =>
            Error.NotFound(
                "ItemAudit.NotFoundById",
                $"Item audit with ID '{id}' not found.");
    }
}
