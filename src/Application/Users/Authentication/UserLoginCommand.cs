using Application.Abstraction.Messaging;

namespace Application.Users.Authentication
{
    public sealed record UserLoginCommand(
        string Username,
        string Password
    ) : ICommand<LoginResponse>;
}
