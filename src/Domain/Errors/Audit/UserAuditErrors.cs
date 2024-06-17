using Domain.Abstraction;

namespace Domain.Errors.Audit
{
    public static class UserAuditErrors
    {
        public static Error UserAuditNotFoundById(int id) =>
            Error.NotFound(
                "UserAudit.NotFoundById",
                $"User audit with ID '{id}' not found.");
    }
}
