using Application.Abstraction.Messaging;
using Application.DTOs.Employee;
using Application.DTOs.Items.ItemResponsibility;
using Domain.Abstraction;
using Domain.Entities;
using Domain.Repositories.Addresses;
using Domain.Repositories.Item;
using Domain.Repositories.Staff;

namespace Application.Employees.Queries.Search.ById
{
    public sealed class GetEmployeeByIdQueryHandler : IQueryHandler<GetEmployeeByIdQuery, EmployeeWithDetailsDto>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IPositionRepository _positionRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IItemResponsibilityRepository _itemResponsibilityRepository;
        private readonly IItemRepository _itemRepository;

        public GetEmployeeByIdQueryHandler(
            IEmployeeRepository employeeRepository,
            IPositionRepository positionRepository,
            IAddressRepository addressRepository,
            IItemResponsibilityRepository itemResponsibilityRepository,
            IItemRepository itemRepository)
        {
            _employeeRepository = employeeRepository;
            _positionRepository = positionRepository;
            _addressRepository = addressRepository;
            _itemResponsibilityRepository = itemResponsibilityRepository;
            _itemRepository = itemRepository;
        }

        public async Task<Result<EmployeeWithDetailsDto>> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
        {
            var employeeResult = await _employeeRepository.GetEmployeeByIdAsync(request.Id);
            if (!employeeResult.IsSuccess)
                return Result<EmployeeWithDetailsDto>.Failure(employeeResult.Error);

            var employee = employeeResult.Value;

            var positionResult = await _positionRepository.GetPositionByIdAsync(employee.PositionId);
            if (!positionResult.IsSuccess)
                return Result<EmployeeWithDetailsDto>.Failure(positionResult.Error);

            var position = positionResult.Value.Name;

            var addressResult = await _addressRepository.GetAddressByIdAsync(employee.AddressId);
            if (!addressResult.IsSuccess)
                return Result<EmployeeWithDetailsDto>.Failure(addressResult.Error);

            var address = addressResult.Value;
            var fullAddress = $"{address.City}, {address.Street}, {address.House}, {address.Building}, {address.Apartment}";

            var itemResponsibilitiesResult = await _itemResponsibilityRepository.GetItemResponsibilitiesByEmployeeIdAsync(employee.Id);
            if (!itemResponsibilitiesResult.IsSuccess)
                return Result<EmployeeWithDetailsDto>.Failure(itemResponsibilitiesResult.Error);

            var itemResponsibilities = new List<ItemResponsibilityWithoutDetails>();
            foreach (var responsibility in itemResponsibilitiesResult.Value)
            {
                var itemResult = await _itemRepository.GetItemByIdAsync(responsibility.ItemId);
                if (!itemResult.IsSuccess)
                    return Result<EmployeeWithDetailsDto>.Failure(itemResult.Error);

                var item = itemResult.Value;
                itemResponsibilities.Add(new ItemResponsibilityWithoutDetails(item.Name, item.Number, responsibility.AssignedAt.ToString("yyyy-MM-dd HH:mm:ss")));
            }

            // Формируем DTO
            var employeeDto = new EmployeeWithDetailsDto(
                employee.Id.ToString(),
                employee.Name,
                employee.Surname,
                employee.Lastname,
                employee.PhoneNumber,
                employee.EmailAddress,
                employee.Sex,
                employee.Age.ToString(),
                position,
                employee.AddressId,
                fullAddress,
                itemResponsibilities
            );

            return Result<EmployeeWithDetailsDto>.Success(employeeDto);
        }
    }
}
