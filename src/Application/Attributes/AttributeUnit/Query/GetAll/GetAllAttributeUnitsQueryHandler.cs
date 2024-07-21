using Application.Abstraction.Messaging;
using Application.DTOs.Attributes.Measurement;
using Domain.Abstraction;
using Domain.Repositories.Item.Attributes;

namespace Application.Attributes.AttributeUnit.Query.GetAll
{
    public class GetAllAttributeUnitsQueryHandler : IQueryHandler<GetAllAttributeUnitsQuery, IEnumerable<AttributeUnitDto>>
    {
        private readonly IAttributeUnitRepository _attributeUnitRepository;

        public GetAllAttributeUnitsQueryHandler(IAttributeUnitRepository attributeUnitRepository)
        {
            _attributeUnitRepository = attributeUnitRepository;
        }

        public async Task<Result<IEnumerable<AttributeUnitDto>>> Handle(GetAllAttributeUnitsQuery request, CancellationToken cancellationToken)
        {
            var attributeUnitsResult = await _attributeUnitRepository.GetAllAttributeUnitsWithEntitiesAsync();
            if (attributeUnitsResult.IsFailure)
                return Result<IEnumerable<AttributeUnitDto>>.Failure(attributeUnitsResult.Error);

            var attributeUnitsDto = attributeUnitsResult.Value.Select(au => new AttributeUnitDto(
                au.Name,
                au.Symbol,
                au.AttributeValueType!.Name,
                au.UnitCategory!.Name
            ));

            return Result<IEnumerable<AttributeUnitDto>>.Success(attributeUnitsDto);
        }
    }
}
