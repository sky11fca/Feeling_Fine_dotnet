using WebApi.Models;

namespace WebApi.Services.Client;

public interface IClientService
{
    Task AddAsync(string username, string email, string phoneNumber);
    Task<ClientDto?> FindAsync(Guid guid);
    Task<List<ClientDto>> Query();
}