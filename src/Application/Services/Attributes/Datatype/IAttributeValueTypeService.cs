using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.Attributes;

namespace Application.Services.Attributes.Datatype
{
    public interface IAttributeValueTypeService
    {
        IEnumerable<AttributeValueTypeDto> GetAllAttributeValueTypes();
    }
}