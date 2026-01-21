using Askly.Application.Interfaces.Auth;

namespace Askly.Infrastructure;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password) 
        => BCrypt.Net.BCrypt.EnhancedHashPassword(password);

    public bool VerifyPassword(string hashedPassword, string password)
        => BCrypt.Net.BCrypt.EnhancedVerify(password, hashedPassword);
    
}