using Domain.Abstraction;
using Domain.Entities;

namespace Domain.Repositories.Addresses
{
    public interface IRegionRepository
    {
        Task<Result<IEnumerable<Region>>> GetAllRegionsAsync();
        Task<Result> AddRegionAsync(Region region);
        Task<Result<Region>> GetRegionByIdAsync(int id);
        Task<Result<Region>> GetRegionByNameAsync(string name);
        Task<Result> UpdateRegionAsync(Region region);
        Task<Result> DeleteRegionAsync(int id);
    }
}
