namespace DotnetApi.Application.Clients.Query;

public record ClientDto(
    Guid Id,
    string Username,
    string Email,
    string PhoneNumber
    );