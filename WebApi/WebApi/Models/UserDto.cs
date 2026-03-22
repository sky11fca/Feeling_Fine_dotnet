namespace WebApi.Models;

public record UserDto(
    Guid Id,
    Guid BusinessId,
    string Username,
    string Email,
    string Password,
    int UserRole,
    DateTime CreatedAt
    );