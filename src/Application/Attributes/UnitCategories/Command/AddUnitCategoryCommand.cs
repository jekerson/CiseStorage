using Application.Abstraction.Messaging;

namespace Application.Attributes.UnitCategories.Command
{
    public record AddUnitCategoryCommand(string Name) : ICommand;

}