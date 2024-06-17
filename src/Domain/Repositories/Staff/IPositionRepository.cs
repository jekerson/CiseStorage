using Domain.Abstraction;
using Domain.Entities;

namespace Domain.Repositories.Staff
{
    public interface IPositionRepository
    {
        Task<Result<IEnumerable<Position>>> GetAllPositionsAsync();
        Task<Result> AddPositionAsync(Position position);
        Task<Result<Position>> GetPositionByIdAsync(int id);
        Task<Result<Position>> GetPositionByNameAsync(string name);
        Task<Result> UpdatePositionAsync(Position position);
        Task<Result> DeletePositionAsync(int id);
    }
}
