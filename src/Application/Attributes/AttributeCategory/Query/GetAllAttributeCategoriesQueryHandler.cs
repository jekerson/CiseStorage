using Application.Abstraction.Messaging;
using Application.DTOs.Attributes;
using Domain.Abstraction;
using Domain.Repositories.Item.Attributes;

namespace Application.Attributes.AttributeCategory.Query
{
    public class GetAllAttributeCategoriesQueryHandler : IQueryHandler<GetAllAttributeCategoriesQuery, IEnumerable<AttributeCategoryDto>>
    {
        private readonly IAttributeCategoryRepository _attributeCategoryRepository;

        public GetAllAttributeCategoriesQueryHandler(IAttributeCategoryRepository attributeCategoryRepository)
        {
            _attributeCategoryRepository = attributeCategoryRepository;
        }

        public async Task<Result<IEnumerable<AttributeCategoryDto>>> Handle(GetAllAttributeCategoriesQuery request, CancellationToken cancellationToken)
        {
            var attributeCategoriesResult = await _attributeCategoryRepository.GetAllAttributeCategoriesAsync();
            if (attributeCategoriesResult.IsFailure)
                return Result<IEnumerable<AttributeCategoryDto>>.Failure(attributeCategoriesResult.Error);

            var attributeCategoriesDto = attributeCategoriesResult.Value.Select(ac => new AttributeCategoryDto(ac.Id, ac.Name));

            return Result<IEnumerable<AttributeCategoryDto>>.Success(attributeCategoriesDto);
        }
    }
}
