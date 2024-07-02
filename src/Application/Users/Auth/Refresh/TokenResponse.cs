namespace Application.Users.Auth.Refresh
{
    public sealed record TokenResponse
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
