using Domain.Abstraction;
using Domain.Entities;

namespace Domain.Repositories.Staff
{
    public interface IUserRepository
    {
        Task<Result<IEnumerable<UserInfo>>> GetAllUsersAsync();
        Task<Result> AddUserAsync(UserInfo userInfo);
        Task<Result<UserInfo>> GetUserByIdAsync(int id);
        Task<Result<UserInfo>> GetUserByUsernameAsync(string username);
        Task<Result> UpdateUserAsync(UserInfo userInfo);
        Task<Result> DeleteUserAsync(int id);
        Task<Result<IEnumerable<UserInfo>>> GetAllDeletedUsersAsync();
        Task<Result<UserInfo>> GetDeletedUserByUsernameAsync(string username);
        Task<Result<UserInfo>> GetDeletedUserByIdAsync(int id);
    }


}
