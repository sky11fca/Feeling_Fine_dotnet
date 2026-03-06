using DotnetApi.Domains.Entities;

namespace DotnetApi.Application.Abstractions;

public interface IClientRepository
{
    Task AddAsync(Client client, CancellationToken cancellationToken = default);
    Task<Client?> FindAsync(Guid id, CancellationToken cancellationToken = default);
    IQueryable<Client> Query();
}