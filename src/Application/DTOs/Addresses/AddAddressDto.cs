namespace Application.DTOs.Addresses
{
    public record AddAddressDto(
        string City,
        string Street,
        string House,
        string? Building,
        string Apartment,
        string RegionName
    );
}