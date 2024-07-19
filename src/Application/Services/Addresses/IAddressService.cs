using Application.DTOs.Addresses;
using Domain.Abstraction;
using Domain.Entities;

namespace Application.Services.Addresses
{
    public interface IAddressService
    {
        Task<Result<Address>> CreateAddressAsync(AddAddressDto addressDto);
        Task<Result<Address>> UpdateAddressAsync(int addressId, AddAddressDto addressDto);
    }
}
