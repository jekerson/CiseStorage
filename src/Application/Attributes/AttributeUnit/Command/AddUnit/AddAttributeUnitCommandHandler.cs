using Application.Abstraction;
using Application.Abstraction.Messaging;
using Domain.Abstraction;
using Domain.Repositories.Item.Attributes;

namespace Application.Attributes.AttributeUnit.Command.AddUnit
{
    public sealed class AddAttributeUnitCommandHandler : ICommandHandler<AddAttributeUnitCommand>
    {
        private readonly IAttributeUnitRepository _attributeUnitRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddAttributeUnitCommandHandler(IAttributeUnitRepository attributeUnitRepository, IUnitOfWork unitOfWork)
        {
            _attributeUnitRepository = attributeUnitRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(AddAttributeUnitCommand request, CancellationToken cancellationToken)
        {
            var attributeUnitDto = request.AttributeUnitDto;

            var attributeUnit = new Domain.Entities.AttributeUnit
            {
                Name = attributeUnitDto.Name,
                Symbol = attributeUnitDto.Symbol,
                UnitCategoryId = attributeUnitDto.UnitCategoryId,
                AttributeValueTypeId = attributeUnitDto.AttributeValueTypeId
            };

            var result = await _attributeUnitRepository.AddAttributeUnitAsync(attributeUnit);
            if (result.IsFailure)
                return Result.Failure(result.Error);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}