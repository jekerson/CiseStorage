using Application.Abstraction.Messaging;

namespace Application.Users.Refresh
{
    public sealed record RefreshTokenCommand(
        int UserId,
        string RefreshToken
    ) : ICommand<TokenResponse>;
}
