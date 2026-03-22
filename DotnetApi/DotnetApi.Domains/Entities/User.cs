using System.Runtime.InteropServices;
using DotnetApi.Domains.Enums;

namespace DotnetApi.Domains.Entities;

public class User
{
    private User()
    {
        
    }

    public static User Create(Guid businessId, string username, string email, string password, string userRole)
    {
        if (businessId == Guid.Empty)
        {
            throw new ArgumentException("Business ID cannot be empty.", nameof(businessId));
        }
        if (string.IsNullOrEmpty(username))
        {
            throw new ArgumentException("Username cannot be null or empty.", nameof(username));
        }

        if (string.IsNullOrEmpty(email))
        {
            throw new ArgumentException("Email cannot be null or empty.", nameof(email));
        }
        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentException("Password cannot be null or empty.", nameof(password));
        }

        var role = SetUserRole(userRole);
        if (role == UserRole.Unknown)
        {
            throw new ArgumentException("Invalid user role provided.", nameof(userRole));
        }

        return new User
        {
            Id = Guid.NewGuid(),
            BusinessId = businessId,
            Username = username,
            Email = email,
            Password = password,
            UserRole = role,
            CreatedAt = DateTime.UtcNow
        };
    }

    private static UserRole SetUserRole(string userRole)
    {
        return userRole switch
        {
            "Admin" => UserRole.Admin, "Employer" => UserRole.Employer, "Employee" => UserRole.Employee, _ => UserRole.Unknown
        };
    }
    
    public Guid Id { get; private set; }
    public Guid BusinessId { get; private set; }
    public string Username { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Password { get; private set; } = string.Empty;
    public UserRole UserRole { get; private set; } = UserRole.Unknown;
    public DateTime CreatedAt { get; private set;}
}