using FluentValidation;

namespace Application.Employees.Commands.DeleteEmployee
{
    public class DeleteEmployeeCommandValidator : AbstractValidator<DeleteEmployeeCommand>
    {
        public DeleteEmployeeCommandValidator()
        {
            RuleFor(x => x.DeleteEmployeeDto.EmployeeId).GreaterThan(0).WithMessage("EmployeeId must be greater than 0.");
            RuleFor(x => x.DeleteEmployeeDto.EmployeeId).GreaterThan(0).WithMessage("UserId must be greater than 0.");
        }
    }
}