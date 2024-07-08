using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Services.Audit.Staff;
using Domain.Abstraction;
using Domain.Entities;
using Domain.Enum;
using Domain.Repositories.Addresses;
using Domain.Repositories.Staff;
using MediatR;

namespace Application.Employees.Commands.AddEmployee
{
public class AddEmployeeCommandHandler : IRequestHandler<AddEmployeeCommand, Result>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IRegionRepository _regionRepository;
        private readonly IPositionRepository _positionRepository;
        private readonly IEmployeeAuditService _employeeAuditService;
        private readonly IUserRepository _userRepository;

        public AddEmployeeCommandHandler(
            IEmployeeRepository employeeRepository,
            IAddressRepository addressRepository,
            IRegionRepository regionRepository,
            IPositionRepository positionRepository,
            IEmployeeAuditService employeeAuditService,
            IUserRepository userRepository)
        {
            _employeeRepository = employeeRepository;
            _addressRepository = addressRepository;
            _regionRepository = regionRepository;
            _positionRepository = positionRepository;
            _employeeAuditService = employeeAuditService;
            _userRepository = userRepository;
        }

        public async Task<Result> Handle(AddEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employeeDto = request.EmployeeDto;

            var userResult = await _userRepository.GetUserByIdAsync(employeeDto.UserId);
            if (!userResult.IsSuccess)
                return Result.Failure(userResult.Error);

            var positionResult = await _positionRepository.GetPositionByNameAsync(employeeDto.PositionName);
            if (!positionResult.IsSuccess)
                return Result.Failure(positionResult.Error);

            var position = positionResult.Value;

            var regionResult = await _regionRepository.GetRegionByNameAsync(employeeDto.AddAddressDto.RegionName);
            if (!regionResult.IsSuccess)
                return Result.Failure(regionResult.Error);

            var region = regionResult.Value;

            var address = new Address
            {
                City = employeeDto.AddAddressDto.City,
                Street = employeeDto.AddAddressDto.Street,
                House = employeeDto.AddAddressDto.House,
                Building = employeeDto.AddAddressDto.Building,
                Apartment = employeeDto.AddAddressDto.Apartment,
                RegionId = region.Id
            };

            var addressResult = await _addressRepository.AddAddressAsync(address);
            if (!addressResult.IsSuccess)
                return Result.Failure(addressResult.Error);

            var employee = new Employee
            {
                Name = employeeDto.Name,
                Surname = employeeDto.Surname,
                Lastname = employeeDto.Lastname,
                PhoneNumber = employeeDto.Phone,
                EmailAddress = employeeDto.Email,
                Sex = employeeDto.Sex,
                Age = employeeDto.Age,
                PositionId = position.Id,
                AddressId = address.Id
            };

            var employeeResult = await _employeeRepository.AddEmployeeAsync(employee);
            if (!employeeResult.IsSuccess)
                return Result.Failure(employeeResult.Error);

            // Add audit log
            var auditResult = await _employeeAuditService.AddEmployeeAuditAsync(ActionType.Insert, employeeDto.UserId, employee.Id);
            if (!auditResult.IsSuccess)
                return Result.Failure(auditResult.Error);

            return Result.Success();
        }
    }
}