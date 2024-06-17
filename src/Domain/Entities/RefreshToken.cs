namespace Domain.Entities
{
    public partial class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserInfoId { get; set; }
        public virtual UserInfo UserInfo { get; set; } = null!;
    }
}
