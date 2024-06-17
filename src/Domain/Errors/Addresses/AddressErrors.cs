using Domain.Abstraction;
using Domain.Entities;

namespace Domain.Errors.Addresses
{
    public static class AddressErrors
    {
        public static Error AddressAlreadyExist(Address address) =>
            Error.Conflict(
                "Address.AlreadyExist",
                $"Address '{address.City}, {address.Street}, {address.House}, {address.Building}, {address.Apartment}' already exists.");

        public static Error AddressNotFoundById(int id) =>
            Error.NotFound(
                "Address.NotFoundById",
                $"Address with ID '{id}' not found.");

        public static Error AddressNotFoundByDetails(string city, string street, string house, string building, string apartment) =>
            Error.NotFound(
                "Address.NotFoundByDetails",
                $"Address '{city}, {street}, {house}, {building}, {apartment}' not found.");
    }
}
