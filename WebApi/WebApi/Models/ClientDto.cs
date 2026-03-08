namespace WebApi.Models;

public record ClientDto(
    Guid Id,
    string Username,
    string Email,
    string PhoneNumber
    );