using System.Net.Http.Json;
using WebApi.Models;
using WebApi.Models.Requests;
using WebApi.Models.Responses;

namespace WebApi.Services.Business;

public class BusinessService(HttpClient httpClient) : IBusinessService
{
    private readonly string BaseUri = "http://localhost:5160/api/v1/business";
    
    public async Task<Guid> AddBusiness(string name, string industry)
    {
        var request = new AddBusinessCommand(name, industry);
        var response = await httpClient.PostAsJsonAsync(BaseUri, request);
        var result = await response.Content.ReadFromJsonAsync<AddBusinessResponse>();
        return result?.Id ?? Guid.Empty;
    }

    public async  Task<List<BusinessDto?>> GetBusinessQuery(string name, string industry)
    {
        return await httpClient.GetFromJsonAsync<List<BusinessDto?>>($"{BaseUri}?name={name}&industry={industry}");
    }
}