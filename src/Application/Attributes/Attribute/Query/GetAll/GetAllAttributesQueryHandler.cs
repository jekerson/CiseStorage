using Application.Abstraction.Messaging;
using Application.DTOs.Attributes;
using Application.DTOs.Attributes.Measurement;
using Application.Services.Attributes.Datatype;
using Domain.Abstraction;
using Domain.Repositories.Item.Attributes;

namespace Application.Attributes.Attribute.Query.GetAll
{
    public class GetAllAttributesQueryHandler : IQueryHandler<GetAllAttributesQuery, IEnumerable<AttributeDto>>
    {
        private readonly IAttributeRepository _attributeRepository;

        private readonly IAttributeValueTypeService _attributeValueTypeService;

        public GetAllAttributesQueryHandler(IAttributeRepository attributeRepository, IAttributeValueTypeService attributeValueTypeService)
        {
            _attributeRepository = attributeRepository;
            _attributeValueTypeService = attributeValueTypeService;
        }

        public async Task<Result<IEnumerable<AttributeDto>>> Handle(GetAllAttributesQuery request, CancellationToken cancellationToken)
        {
            var result = await _attributeRepository.GetAllAttributesWithEntities();
            if (!result.IsSuccess)
                return Result<IEnumerable<AttributeDto>>.Failure(result.Error);

            var attributeValueTypeDtoMap = _attributeValueTypeService.GetAllAttributeValueTypes()
                .ToDictionary(dto => dto.Id);

            var attributeDto = result.Value.Select(a => new AttributeDto(
                a.Id,
                a.Name,
                a.IsRequired,
                a.AttributeCategory.Name,
                new AttributeUnitDto(
                    a.AttributeUnit.Name,
                    a.AttributeUnit.Symbol,
                    a.AttributeUnit.UnitCategory.Name,
                    attributeValueTypeDtoMap[a.AttributeUnit.AttributeValueType.Id].DatatypeDisplayName
                )
            ));

            return Result<IEnumerable<AttributeDto>>.Success(attributeDto);
        }
    }
}
