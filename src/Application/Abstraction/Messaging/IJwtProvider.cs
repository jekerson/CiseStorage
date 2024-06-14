using Domain.Entities;

namespace Application.Abstraction.Messaging
{
    public interface IJwtProvider
    {
        string GenerateToken(UserInfo user);
        string GenerateRefreshToken();

    }
}
