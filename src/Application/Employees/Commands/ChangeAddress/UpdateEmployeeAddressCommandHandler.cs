using Application.Abstraction.Messaging;
using Domain.Abstraction;
using Domain.Repositories.Addresses;
using Domain.Repositories.Staff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Employees.Commands.ChangeAddress
{
    public class UpdateEmployeeAddressCommandHandler : ICommandHandler<UpdateEmployeeAddressCommand>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IRegionRepository _regionRepository;

        public UpdateEmployeeAddressCommandHandler(
            IEmployeeRepository employeeRepository,
            IAddressRepository addressRepository,
            IRegionRepository regionRepository)
        {
            _employeeRepository = employeeRepository;
            _addressRepository = addressRepository;
            _regionRepository = regionRepository;
        }

        public async Task<Result> Handle(UpdateEmployeeAddressCommand request, CancellationToken cancellationToken)
        {
            var employeeResult = await _employeeRepository.GetEmployeeByIdAsync(request.EmployeeId);
            if (!employeeResult.IsSuccess)
                return Result.Failure(employeeResult.Error);

            var employee = employeeResult.Value;

            var regionResult = await _regionRepository.GetRegionByNameAsync(request.NewAddress.RegionName);
            if (!regionResult.IsSuccess)
                return Result.Failure(regionResult.Error);

            var region = regionResult.Value;

            var address = await _addressRepository.GetAddressByIdAsync(employee.AddressId);
            if (!address.IsSuccess)
                return Result.Failure(address.Error);

            var currentAddress = address.Value;
            currentAddress.City = request.NewAddress.City;
            currentAddress.Street = request.NewAddress.Street;
            currentAddress.House = request.NewAddress.House;
            currentAddress.Building = request.NewAddress.Building;
            currentAddress.Apartment = request.NewAddress.Apartment;
            currentAddress.RegionId = region.Id;

            var updateAddressResult = await _addressRepository.UpdateAddressAsync(currentAddress);
            if (!updateAddressResult.IsSuccess)
                return Result.Failure(updateAddressResult.Error);

            return Result.Success();
        }
    }
}
