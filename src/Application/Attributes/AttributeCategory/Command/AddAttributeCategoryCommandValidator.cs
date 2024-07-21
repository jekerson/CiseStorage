using FluentValidation;

namespace Application.Attributes.AttributeCategory.Command
{
    public class AddAttributeCategoryCommandValidator : AbstractValidator<AddAttributeCategoryCommand>
    {
        public AddAttributeCategoryCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name cannot exceed 50 characters.");
        }
    }
}
