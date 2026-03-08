using DotnetApi.Application.Abstractions;
using DotnetApi.Domains.Entities;
using DotnetApi.Infrastructure.Persistance;

namespace DotnetApi.Infrastructure.Repository;

public class ReplyRepository(ApplicationDbContext context) : IReplyRepository
{
    public async Task AddAsync(Reply reply, CancellationToken cancellationToken = default)
    {
        await context.Replies.AddAsync(reply, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Reply?> FindReply(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Replies.FindAsync(cancellationToken, id);
    }

    public IQueryable<Reply> Query()
    {
        return context.Replies;
    }
}