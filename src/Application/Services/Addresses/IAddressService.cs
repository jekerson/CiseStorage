using Application.DTOs.Addresses;
using Domain.Abstraction;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Addresses
{
    public interface IAddressService
    {
        Task<Result<Address>> CreateAddressAsync(AddAddressDto addressDto);
        Task<Result<Address>> UpdateAddressAsync(int addressId, AddAddressDto addressDto);
    }
}
