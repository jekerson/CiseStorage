using Application.Abstraction.Messaging;
using Application.Services.Password;
using Domain.Abstraction;
using Domain.Entities;
using Domain.Errors.Staff;
using Domain.Repositories.Role_permission;
using Domain.Repositories.Staff;

namespace Application.Users.Authentication
{
    public sealed class UserLoginCommandHandler : ICommandHandler<UserLoginCommand, LoginResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly IJwtProvider _jwtProvider;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public UserLoginCommandHandler(
            IUserRepository userRepository,
            IPasswordService passwordService,
            IJwtProvider jwtProvider,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _jwtProvider = jwtProvider;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<Result<LoginResponse>> Handle(UserLoginCommand request, CancellationToken cancellationToken)
        {
            var userResult = await _userRepository.GetUserByUsernameAsync(request.Username);
            if (!userResult.IsSuccess)
                return Result<LoginResponse>.Failure(userResult.Error);

            var user = userResult.Value;

            if (!_passwordService.VerifyPassword(request.Password, user.HashedPassword, user.Salt))
                return Result<LoginResponse>.Failure(UserErrors.InvalidCredentials);

            var accessToken = _jwtProvider.GenerateToken(user);
            var refreshToken = _jwtProvider.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(14),
                CreatedAt = DateTime.UtcNow,
                UserInfoId = user.Id
            };

            var addRefreshTokenResult = await _refreshTokenRepository.AddRefreshTokenAsync(refreshTokenEntity);
            if (!addRefreshTokenResult.IsSuccess)
                return Result<LoginResponse>.Failure(addRefreshTokenResult.Error);

            var response = new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            return Result<LoginResponse>.Success(response);
        }
    }
}
