using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstraction;
using Application.Abstraction.Messaging;
using Domain.Abstraction;
using Domain.Entities;
using Domain.Repositories.Item.Attributes;

namespace Application.Attributes.UnitCategories.Command
{
    public sealed class AddUnitCategoryCommandHandler : ICommandHandler<AddUnitCategoryCommand>
    {
        private readonly IUnitCategoryRepository _unitCategoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddUnitCategoryCommandHandler(
            IUnitCategoryRepository unitCategoryRepository,
            IUnitOfWork unitOfWork)
        {
            _unitCategoryRepository = unitCategoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(AddUnitCategoryCommand request, CancellationToken cancellationToken)
        {
            var unitCategory = new UnitCategory { Name = request.Name };

            var result = await _unitCategoryRepository.AddUnitCategoryAsync(unitCategory);
            if (result.IsFailure)
            {
                return Result.Failure(result.Error);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}