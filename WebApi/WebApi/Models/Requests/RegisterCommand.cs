namespace WebApi.Models.Requests;

public class RegisterCommand
{
    public string Username { get; set; }
    public Guid BusinessId { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string UserRole { get; set; }
}