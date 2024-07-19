using FluentValidation;

namespace Application.Attributes.UnitCategories.Command
{
    public class AddUnitCategoryCommandValidator : AbstractValidator<AddUnitCategoryCommand>
    {
        public AddUnitCategoryCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name cannot exceed 50 characters.");
        }
    }
}