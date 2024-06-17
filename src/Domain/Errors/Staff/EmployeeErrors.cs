using Domain.Abstraction;

namespace Domain.Errors.Staff
{
    public static class EmployeeErrors
    {
        public static Error EmployeeAlreadyExistByEmail(string email) =>
            Error.Conflict(
                "Employee.AlreadyExistByEmail",
                $"Employee with email '{email}' already exists.");

        public static Error EmployeeAlreadyExistByPhoneNumber(string phoneNumber) =>
            Error.Conflict(
                "Employee.AlreadyExistByPhoneNumber",
                $"Employee with phone number '{phoneNumber}' already exists.");

        public static Error EmployeeNotFoundById(int employeeId) =>
            Error.NotFound(
                "Employee.NotFoundById",
                $"Employee with ID '{employeeId}' not found.");

        public static Error EmployeeNotFoundByEmail(string email) =>
            Error.NotFound(
                "Employee.NotFoundByEmail",
                $"Employee with email '{email}' not found.");

        public static Error EmployeeNotFoundByPhoneNumber(string phoneNumber) =>
            Error.NotFound(
                "Employee.NotFoundByPhoneNumber",
                $"Employee with phone number '{phoneNumber}' not found.");
    }

}
