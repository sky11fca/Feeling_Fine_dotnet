using WebApi.Models;
using WebApi.Models.Requests;

namespace WebApi.Services.Authentication;

public interface IAuthenticationService
{
    Task<string?> Login(string email, string password);
    Task<UserDto> Register(RegisterCommand command);
}