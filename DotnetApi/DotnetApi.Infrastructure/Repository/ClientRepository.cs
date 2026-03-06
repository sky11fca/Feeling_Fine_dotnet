using DotnetApi.Application.Abstractions;
using DotnetApi.Domains.Entities;
using DotnetApi.Infrastructure.Persistance;

namespace DotnetApi.Infrastructure.Repository;

public class ClientRepository(ApplicationDbContext context) : IClientRepository
{
    public async Task AddAsync(Client client, CancellationToken cancellationToken = default)
    {
        await context.Clients.AddAsync(client, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Client?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Clients.FindAsync(new object[] { id }, cancellationToken);
    }

    public IQueryable<Client> Query()
    {
        return context.Clients;
    }
}