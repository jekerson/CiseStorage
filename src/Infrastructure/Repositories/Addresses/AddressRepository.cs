using Domain.Abstraction;
using Domain.Entities;
using Domain.Errors.Addresses;
using Domain.Repositories.Addresses;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Repositories.Addresses
{
    public class AddressRepository : IAddressRepository
    {
        private readonly SiceDbContext _dbContext;
        private readonly IMemoryCache _cache;
        private const string AddressesCacheKey = "addressesCache";
        private const string AddressCacheKeyPrefix = "addressCache_";

        public AddressRepository(SiceDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<Result<IEnumerable<Address>>> GetAllAddressesAsync()
        {
            if (!_cache.TryGetValue(AddressesCacheKey, out IEnumerable<Address> addresses))
            {
                addresses = await _dbContext.Addresses.AsNoTracking().ToListAsync();
                _cache.Set(AddressesCacheKey, addresses, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1)));
            }
            return Result<IEnumerable<Address>>.Success(addresses);
        }

        public async Task<Result<int>> AddAddressAsync(Address address)
        {
            if (await IsAddressExistAsync(address))
                return Result<int>.Failure(AddressErrors.AddressAlreadyExist(address));

            await _dbContext.Addresses.AddAsync(address);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(AddressesCacheKey);
            return Result<int>.Success(address.Id);
        }

        public async Task<Result<Address>> GetAddressByIdAsync(int id)
        {
            var cacheKey = $"{AddressCacheKeyPrefix}{id}";
            if (!_cache.TryGetValue(cacheKey, out Address address))
            {
                address = await _dbContext.Addresses.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
                if (address == null)
                    return Result<Address>.Failure(AddressErrors.AddressNotFoundById(id));

                _cache.Set(cacheKey, address, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1)));
            }
            return Result<Address>.Success(address);
        }

        public async Task<Result<Address>> GetAddressByDetailsAsync(string city, string street, string house, string building, string apartment)
        {
            var address = await _dbContext.Addresses.AsNoTracking().FirstOrDefaultAsync(a =>
                a.City == city && a.Street == street && a.House == house && a.Building == building && a.Apartment == apartment);
            if (address == null)
                return Result<Address>.Failure(AddressErrors.AddressNotFoundByDetails(city, street, house, building, apartment));

            return Result<Address>.Success(address);
        }

        public async Task<Result> UpdateAddressAsync(Address address)
        {
            var existingAddress = await _dbContext.Addresses.FindAsync(address.Id);
            if (existingAddress == null)
                return Result.Failure(AddressErrors.AddressNotFoundById(address.Id));

            if (await IsAddressExistAsync(address))
                return Result.Failure(AddressErrors.AddressAlreadyExist(address));

            _dbContext.Entry(existingAddress).CurrentValues.SetValues(address);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(AddressesCacheKey);
            _cache.Remove($"{AddressCacheKeyPrefix}{address.Id}");
            return Result.Success();
        }

        public async Task<Result> DeleteAddressAsync(int id)
        {
            var address = await _dbContext.Addresses.FindAsync(id);
            if (address == null)
                return Result.Failure(AddressErrors.AddressNotFoundById(id));

            _dbContext.Addresses.Remove(address);
            await _dbContext.SaveChangesAsync();
            _cache.Remove(AddressesCacheKey);
            _cache.Remove($"{AddressCacheKeyPrefix}{id}");
            return Result.Success();
        }

        private async Task<bool> IsAddressExistAsync(Address address)
        {
            return await _dbContext.Addresses.AnyAsync(a => a.Id == address.Id);
        }
    }
}
