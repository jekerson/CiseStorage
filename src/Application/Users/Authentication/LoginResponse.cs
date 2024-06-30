namespace Application.Users.Authentication
{
    public sealed record LoginResponse
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
