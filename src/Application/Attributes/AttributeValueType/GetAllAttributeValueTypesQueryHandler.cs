using Application.Abstraction.Messaging;
using Application.DTOs.Attributes;
using Application.Services.Attributes.Datatype;
using Domain.Abstraction;

namespace Application.Attributes.AttributeValueType
{
    public class GetAllAttributeValueTypesQueryHandler : IQueryHandler<GetAllAttributeValueTypesQuery, IEnumerable<AttributeValueTypeDto>>
    {
        private readonly IAttributeValueTypeService _attributeValueTypeService;

        public GetAllAttributeValueTypesQueryHandler(IAttributeValueTypeService attributeValueTypeService)
        {
            _attributeValueTypeService = attributeValueTypeService;
        }

        public Task<Result<IEnumerable<AttributeValueTypeDto>>> Handle(GetAllAttributeValueTypesQuery request, CancellationToken cancellationToken)
        {
            var attributeValueTypes = _attributeValueTypeService.GetAllAttributeValueTypes();
            return Task.FromResult(Result<IEnumerable<AttributeValueTypeDto>>.Success(attributeValueTypes));
        }
    }
}