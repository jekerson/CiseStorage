using Application.Abstraction.Messaging;

namespace Application.Users.Auth.Refresh
{
    public sealed record RefreshTokenCommand(
        int UserId,
        string RefreshToken
    ) : ICommand<TokenResponse>;
}
