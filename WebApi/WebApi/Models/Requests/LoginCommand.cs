namespace WebApi.Models.Requests;

public class LoginCommand
{
    public string Email { get; set; }
    public string Password { get; set; }
}