using Domain.Abstraction;
using Domain.Entities;

namespace Domain.Repositories.Role_permission
{
    public interface IRefreshTokenRepository
    {
        Task<Result<IEnumerable<RefreshToken>>> GetAllRefreshTokensAsync();
        Task<Result> AddRefreshTokenAsync(RefreshToken refreshToken);
        Task<Result<RefreshToken>> GetRefreshTokenByIdAsync(int id);
        Task<Result<RefreshToken>> GetRefreshTokenByTokenAsync(string token);
        Task<Result> UpdateRefreshTokenAsync(RefreshToken refreshToken);
        Task<Result> DeleteRefreshTokenAsync(int id);
    }
}
