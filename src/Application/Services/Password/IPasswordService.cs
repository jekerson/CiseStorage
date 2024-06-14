namespace Application.Services.Password
{
    public interface IPasswordService
    {
        string GenerateSalt();
        string HashPassword(string plainTextPassword, string salt);
        bool VerifyPassword(string plainTextPassword, string hashedPassword, string salt);
    }
}
