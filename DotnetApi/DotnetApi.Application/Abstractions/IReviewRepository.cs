using DotnetApi.Domains.Entities;

namespace DotnetApi.Application.Abstractions;

public interface IReviewRepository
{
    Task AddAsync(Review review, CancellationToken cancellationToken = default);
    Task<Review?> FindAsync(Guid id, CancellationToken cancellationToken = default);
    IQueryable<Review> Query();
}