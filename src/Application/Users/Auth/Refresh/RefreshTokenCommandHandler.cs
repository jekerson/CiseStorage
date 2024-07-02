using Application.Abstraction.Messaging;
using Domain.Abstraction;
using Domain.Errors;
using Domain.Repositories.Role_permission;
using Domain.Repositories.Staff;

namespace Application.Users.Auth.Refresh
{
    public sealed class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, TokenResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtProvider _jwtProvider;

        public RefreshTokenCommandHandler(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IJwtProvider jwtProvider)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _jwtProvider = jwtProvider;
        }

        public async Task<Result<TokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var userResult = await _userRepository.GetUserByIdAsync(request.UserId);
            if (!userResult.IsSuccess)
                return Result<TokenResponse>.Failure(userResult.Error);

            var user = userResult.Value;

            var refreshTokenResult = await _refreshTokenRepository.GetRefreshTokenByTokenAsync(request.RefreshToken);
            if (!refreshTokenResult.IsSuccess)
                return Result<TokenResponse>.Failure(refreshTokenResult.Error);

            var refreshToken = refreshTokenResult.Value;

            if (refreshToken.UserInfoId != user.Id || refreshToken.ExpiresAt < DateTime.UtcNow)
                return Result<TokenResponse>.Failure(RefreshTokenErrors.TokenInvalid);

            var newAccessToken = _jwtProvider.GenerateToken(user);
            var newRefreshToken = _jwtProvider.GenerateRefreshToken();

            refreshToken.Token = newRefreshToken;
            refreshToken.ExpiresAt = DateTime.UtcNow.AddDays(14);
            await _refreshTokenRepository.UpdateRefreshTokenAsync(refreshToken);

            return Result<TokenResponse>.Success(new TokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }
    }
}
