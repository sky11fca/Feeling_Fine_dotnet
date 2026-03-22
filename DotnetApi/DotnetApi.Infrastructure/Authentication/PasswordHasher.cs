using DotnetApi.Application.Abstractions;
using BC = BCrypt.Net.BCrypt;

namespace DotnetApi.Infrastructure.Authentication;

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        return BC.HashPassword(password);
    }

    public bool Verify(string password, string hashedPassword)
    {
        return BC.Verify(password, hashedPassword);
    }
}