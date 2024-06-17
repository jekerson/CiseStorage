using Domain.Abstraction;

namespace Domain.Errors.Audit
{
    public static class EmployeeAuditErrors
    {
        public static Error EmployeeAuditNotFoundById(int id) =>
            Error.NotFound(
                "EmployeeAudit.NotFoundById",
                $"Employee audit with ID '{id}' not found.");
    }
}
