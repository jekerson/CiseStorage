using Application.Abstraction.Messaging;

namespace Application.Users.Auth.Login
{
    public sealed record UserLoginCommand(
        string Username,
        string Password
    ) : ICommand<LoginResponse>;
}
