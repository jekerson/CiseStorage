using Application.Abstraction.Messaging;
using Application.DTOs.Attributes;
using Domain.Abstraction;
using Domain.Repositories.Item.Attributes;

namespace Application.Attributes.UnitCategories.Query
{
    public class GetAllUnitCategoriesQueryHandler : IQueryHandler<GetAllUnitCategoriesQuery, IEnumerable<UnitCategoryDto>>
    {
        private readonly IUnitCategoryRepository _unitCategoryRepository;

        public GetAllUnitCategoriesQueryHandler(IUnitCategoryRepository unitCategoryRepository)
        {
            _unitCategoryRepository = unitCategoryRepository;
        }

        public async Task<Result<IEnumerable<UnitCategoryDto>>> Handle(GetAllUnitCategoriesQuery request, CancellationToken cancellationToken)
        {
            var unitCategoriesResult = await _unitCategoryRepository.GetAllUnitCategoriesAsync();
            if (unitCategoriesResult.IsFailure)
                return Result<IEnumerable<UnitCategoryDto>>.Failure(unitCategoriesResult.Error);

            var unitCategories = unitCategoriesResult.Value.Select(uc => new UnitCategoryDto(
                uc.Id,
                uc.Name
            ));

            return Result<IEnumerable<UnitCategoryDto>>.Success(unitCategories);
        }
    }
}