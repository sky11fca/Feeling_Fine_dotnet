using WebApi.Models;

namespace WebApi.Services.Business;

public interface IBusinessService
{
    public Task<Guid> AddBusiness(string name, string industry);
    public Task<List<BusinessDto?>> GetBusinessQuery(string name, string industry);
}