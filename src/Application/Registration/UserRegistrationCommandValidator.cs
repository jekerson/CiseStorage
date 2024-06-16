using FluentValidation;

namespace Application.Registration
{
    public class UserRegistrationCommandValidator : AbstractValidator<UserRegistrationCommand>
    {
        public UserRegistrationCommandValidator()
        {
            RuleFor(x => x.EmployeeId)
                .GreaterThan(0).WithMessage("EmployeeId must be greater than 0.")
                .WithSeverity(Severity.Warning);

            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.")
                .Length(3, 50).WithMessage("Username must be between 3 and 50 characters.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

            RuleFor(x => x.RoleId).NotNull()
                .GreaterThan(0).WithMessage("RoleId must be greater than 0.");
        }
    }
}
