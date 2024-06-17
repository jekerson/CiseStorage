using Domain.Abstraction;

namespace Domain.Errors.Audit
{
    public static class ItemResponsibilityAuditErrors
    {
        public static Error ItemResponsibilityAuditNotFoundById(int id) =>
            Error.NotFound(
                "ItemResponsibilityAudit.NotFoundById",
                $"Item responsibility audit with ID '{id}' not found.");
    }
}
