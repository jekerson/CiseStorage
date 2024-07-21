using Application.Abstraction;
using Application.Abstraction.Messaging;
using Domain.Abstraction;
using Domain.Repositories.Item.Attributes;

namespace Application.Attributes.AttributeCategory.Command
{
    public sealed class AddAttributeCategoryCommandHandler : ICommandHandler<AddAttributeCategoryCommand>
    {
        private readonly IAttributeCategoryRepository _attributeCategoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddAttributeCategoryCommandHandler(
            IAttributeCategoryRepository attributeCategoryRepository,
            IUnitOfWork unitOfWork)
        {
            _attributeCategoryRepository = attributeCategoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(AddAttributeCategoryCommand request, CancellationToken cancellationToken)
        {
            var attributeCategory = new Domain.Entities.AttributeCategory { Name = request.Name };

            var result = await _attributeCategoryRepository.AddAttributeCategoryAsync(attributeCategory);
            if (result.IsFailure)
                return Result.Failure(result.Error);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
