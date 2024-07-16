using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstraction.Messaging;

namespace Application.Attributes.UnitCategories.Command
{
    public record AddUnitCategoryCommand(string Name) : ICommand;

}