using Domain.Abstraction;
using Domain.Entities;

namespace Domain.Repositories.Addresses
{
    public interface IAddressRepository
    {
        Task<Result<IEnumerable<Address>>> GetAllAddressesAsync();
        Task<Result> AddAddressAsync(Address address);
        Task<Result<Address>> GetAddressByIdAsync(int id);
        Task<Result<Address>> GetAddressByDetailsAsync(string city, string street, string house, string building, string apartment);
        Task<Result> UpdateAddressAsync(Address address);
        Task<Result> DeleteAddressAsync(int id);
    }
}
