using System.Net.Http.Json;
using WebApi.Models;
using WebApi.Models.Requests;

namespace WebApi.Services.Client;

public class ClientService(HttpClient client) : IClientService
{
    private readonly string BaseUri = "http://localhost:5160/api/v1/client";
    
    public async Task AddAsync(string username, string email, string phoneNumber)
    {
        var req = new AddClientCommand(username, email, phoneNumber);
        await client.PostAsJsonAsync(BaseUri, req);
    }

    public async Task<ClientDto?> FindAsync(Guid guid)
    {
        var req = client.GetFromJsonAsync<ClientDto?>($"{BaseUri}/{guid}");
        return await req;
    }

    public async Task<List<ClientDto>> Query()
    {
        return await client.GetFromJsonAsync<List<ClientDto>>(BaseUri);
    }
}