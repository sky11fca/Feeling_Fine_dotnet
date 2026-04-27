using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using WebApi.Models;
using WebApi.Models.Requests;

namespace WebApi.Services.Authentication;

public class AuthenticationService(HttpClient client) : IAuthenticationService
{

    private readonly string BaseUrl = configuration["ApiUrl"]?.TrimEnd("/") + "/api/Authentication/";
    
    public async Task<string?> Login(string email, string password)
    {
        var request = new LoginCommand
        {
            Email = email,
            Password = password
        };
        var response = await client.PostAsJsonAsync($"{BaseUrl}login", request);
        
        if (!response.IsSuccessStatusCode)
        {
            return null; // Or throw an exception/handle the error as needed
        }
        
        var token = await response.Content.ReadAsStringAsync();
        return token;
    }

    public async Task<UserDto> Register(RegisterCommand command)
    {
        var response = await client.PostAsJsonAsync($"{BaseUrl}register", command);
        
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<UserDto>();
    }
}