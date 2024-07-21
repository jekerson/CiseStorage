using Application.Abstraction.Messaging;

namespace Application.Attributes.AttributeCategory.Command
{
    public record AddAttributeCategoryCommand(string Name) : ICommand;

}
