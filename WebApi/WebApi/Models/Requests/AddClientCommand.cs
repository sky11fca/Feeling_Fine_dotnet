namespace WebApi.Models.Requests;

public record AddClientCommand(
    string Username,
    string Email,
    string PhoneNumber
    );