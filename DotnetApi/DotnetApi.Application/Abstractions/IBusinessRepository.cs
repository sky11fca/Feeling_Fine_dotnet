using DotnetApi.Domains.Entities;

namespace DotnetApi.Application.Abstractions;

public interface IBusinessRepository
{
    Task AddAsync(Business business, CancellationToken cancellationToken = default);
    Task<Business?> FindAsync(Guid id, CancellationToken cancellationToken = default);
    IQueryable<Business> Query();
}