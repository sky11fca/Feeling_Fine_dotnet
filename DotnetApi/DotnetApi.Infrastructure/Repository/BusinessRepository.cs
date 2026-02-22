using DotnetApi.Application.Abstractions;
using DotnetApi.Domains.Entities;
using DotnetApi.Infrastructure.Persistance;

namespace DotnetApi.Infrastructure.Repository;

public class BusinessRepository(ApplicationDbContext context) : IBusinessRepository
{
    public async Task AddAsync(Business business, CancellationToken cancellationToken = default)
    {
        await context.Businesses.AddAsync(business, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Business?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Businesses.FindAsync(new object[] { id }, cancellationToken);
    }

    public IQueryable<Business> Query()
    {
        return context.Businesses;
    }
}