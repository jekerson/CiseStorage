using FluentValidation;

namespace Application.Attributes.AttributeUnit.Command.AddUnit
{
    public class AddAttributeUnitCommandValidator : AbstractValidator<AddAttributeUnitCommand>
    {
        public AddAttributeUnitCommandValidator()
        {
            RuleFor(x => x.AttributeUnitDto.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name cannot exceed 50 characters.");

            RuleFor(x => x.AttributeUnitDto.Symbol)
                .NotEmpty().WithMessage("Symbol is required.")
                .MaximumLength(10).WithMessage("Symbol cannot exceed 10 characters.");

            RuleFor(x => x.AttributeUnitDto.UnitCategoryId)
                .GreaterThan(0).WithMessage("UnitCategoryId must be a positive number.");

            RuleFor(x => x.AttributeUnitDto.AttributeValueTypeId)
                .GreaterThan(0).WithMessage("AttributeValueTypeId must be a positive number.");
        }
    }
}