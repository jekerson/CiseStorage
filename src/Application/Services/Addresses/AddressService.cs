using Application.DTOs.Addresses;
using Domain.Abstraction;
using Domain.Entities;
using Domain.Repositories.Addresses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Addresses
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly IRegionRepository _regionRepository;

        public AddressService(IAddressRepository addressRepository, IRegionRepository regionRepository)
        {
            _addressRepository = addressRepository;
            _regionRepository = regionRepository;
        }

        public async Task<Result<Address>> CreateAddressAsync(AddAddressDto addressDto)
        {
            var regionResult = await _regionRepository.GetRegionByNameAsync(addressDto.RegionName);
            if (!regionResult.IsSuccess)
                return Result<Address>.Failure(regionResult.Error);

            var region = regionResult.Value;
            var address = new Address
            {
                City = addressDto.City,
                Street = addressDto.Street,
                House = addressDto.House,
                Building = addressDto.Building,
                Apartment = addressDto.Apartment,
                RegionId = region.Id
            };

            var addressResult = await _addressRepository.AddAddressAsync(address);
            if (!addressResult.IsSuccess)
                return Result<Address>.Failure(addressResult.Error);

            return Result<Address>.Success(address);
        }

        public async Task<Result<Address>> UpdateAddressAsync(int addressId, AddAddressDto addressDto)
        {
            var regionResult = await _regionRepository.GetRegionByNameAsync(addressDto.RegionName);
            if (!regionResult.IsSuccess)
                return Result<Address>.Failure(regionResult.Error);

            var region = regionResult.Value;
            var addressResult = await _addressRepository.GetAddressByIdAsync(addressId);
            if (!addressResult.IsSuccess)
                return Result<Address>.Failure(addressResult.Error);

            var address = addressResult.Value;
            address.City = addressDto.City;
            address.Street = addressDto.Street;
            address.House = addressDto.House;
            address.Building = addressDto.Building;
            address.Apartment = addressDto.Apartment;
            address.RegionId = region.Id;

            var updateResult = await _addressRepository.UpdateAddressAsync(address);
            if (!updateResult.IsSuccess)
                return Result<Address>.Failure(updateResult.Error);

            return Result<Address>.Success(address);
        }
    }
}
