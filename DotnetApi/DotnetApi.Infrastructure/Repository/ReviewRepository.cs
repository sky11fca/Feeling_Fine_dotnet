using DotnetApi.Application.Abstractions;
using DotnetApi.Domains.Entities;
using DotnetApi.Infrastructure.Persistance;

namespace DotnetApi.Infrastructure.Repository;

public class ReviewRepository(ApplicationDbContext context) : IReviewRepository
{
    public async Task AddAsync(Review review, CancellationToken cancellationToken = default)
    {
        await context.Reviews.AddAsync(review, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Review?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Reviews.FindAsync(cancellationToken, id);
    }

    public IQueryable<Review> Query()
    {
        return context.Reviews;
    }
}