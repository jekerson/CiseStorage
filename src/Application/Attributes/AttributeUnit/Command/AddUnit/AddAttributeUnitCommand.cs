using Application.Abstraction.Messaging;
using Application.DTOs.Attributes.Measurement;

namespace Application.Attributes.AttributeUnit.Command.AddUnit
{
    public record AddAttributeUnitCommand(AddAttributeUnitDto AttributeUnitDto) : ICommand;

}